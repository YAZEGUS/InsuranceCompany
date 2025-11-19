using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Persistence;

/// <summary>
/// Implements the IRepository interface for storing data in a JSON file.
/// Provides basic CRUD operations and ensures data is loaded/saved to a file.
/// </summary>
/// <typeparam name="T">The type of the entity, must inherit from BaseEntity.</typeparam>
public class JsonRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly string _filePath;
    private List<T> _items;
    // Об'єкт для блокування (lock) для забезпечення потокобезпеки
    private readonly object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the JsonRepository.
    /// Loads existing data from the specified JSON file path on creation.
    /// </summary>
    public JsonRepository(string filePath)
    {
        _filePath = filePath;
        LoadData();
    }

    /// <summary>Ы
    /// Loads data from the JSON file into the in-memory list.
    /// </summary>
    private void LoadData()
    {
        lock (_lock)
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                }
                else
                {
                    _items = new List<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Persistence ERROR] Failed to load data from {_filePath}: {ex.Message}");
                _items = new List<T>();
            }
        }
    }

    /// <summary>
    /// Saves the current in-memory list to the JSON file.
    /// </summary>
    private void SaveData()
    {
        lock (_lock)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_items, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL PERSISTENCE ERROR] Failed to save data to {_filePath}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Generates the next available unique ID.
    /// </summary>
    private int GenerateNewId()
    {
        if (_items.Any())
        {
            return _items.Max(item => item.Id) + 1;
        }
        return 1;
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    public void Add(T entity)
    {
        lock (_lock)
        {
            entity.Id = GenerateNewId();
            _items.Add(entity);
            SaveData();
        }
    }

    /// <summary>
    /// Gets all entities from the repository.
    /// </summary>
    public List<T> GetAll()
    {
        lock (_lock)
        {
            // Повертаємо копію
            return new List<T>(_items);
        }
    }

    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    public T? GetById(int id)
    {
        lock (_lock)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }
    }

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    public bool Update(T entity)
    {
        lock (_lock)
        {
            var existingItem = _items.FirstOrDefault(item => item.Id == entity.Id);
            if (existingItem != null)
            {
                int index = _items.IndexOf(existingItem);
                _items[index] = entity;
                SaveData();
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    public bool Delete(int id)
    {
        lock (_lock)
        {
            int initialCount = _items.Count;
            _items.RemoveAll(item => item.Id == id);
            
            if (_items.Count < initialCount)
            {
                SaveData();
                return true;
            }
            return false;
        }
    }
}