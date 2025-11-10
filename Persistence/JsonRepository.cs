using Domain;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Persistence;

/// <summary>
/// A generic repository implementation that stores data in JSON files.
/// Implements the IRepository interface.
/// </summary>
/// <typeparam name="T">The entity type, must inherit from BaseEntity.</typeparam>
public class JsonRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly string _filepath;
    private List<T> _items;

    /// <summary>
    /// Initializes a new instance of the JsonRepository.
    /// Loads data from the specified file path upon creation.
    /// </summary>
    /// <param name="filepath">The path to the .json file (e.g., "clients.json").</param>
    public JsonRepository(string filepath)
    {
        _filepath = filepath;
        _items = new List<T>();
        LoadData();
    }

    /// <summary>
    /// Loads data from the JSON file into the in-memory list (_items).
    /// If the file doesn't exist, it starts with an empty list.
    /// </summary>
    private void LoadData()
    {
        if (!File.Exists(_filepath))
        {
            return;
        }
        
        var json = File.ReadAllText(_filepath);
        if (string.IsNullOrEmpty(json))
        {
            return;
        }
        var loadeditems = JsonConvert.DeserializeObject<List<T>>(json);
        
        if(loadeditems != null)
        {
            _items = loadeditems;
        }
    }
    
    /// <summary>
    /// Saves the current in-memory list (_items) back to the JSON file.
    /// Formats the JSON for readability.
    /// </summary>
    private void SaveData()
    {
        var json = JsonConvert.SerializeObject(_items, Formatting.Indented);
        File.WriteAllText(_filepath, json);
    }
    public List<T> GetAll()
    {
        return _items;
    }
    public T GetById(int id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This implementation automatically generates a new Id
    /// by finding the current maximum Id and adding 1.
    /// </remarks>
    public void Add(T entity)
    { 
        int newItemId = _items.Count > 0 ? _items.Max(item => item.Id) + 1 : 1;
        entity.Id = newItemId;
        _items.Add(entity);
        SaveData();
    }

    /// <inheritdoc />
    /// <remarks>
    /// This implementation safely checks if the item exists before updating.
    /// </remarks>
    public void Update(T entity)
    {
        int entityIndex = _items.FindIndex(item => item.Id == entity.Id);
        if (entityIndex >= 0) // Use >= 0 for clarity, != -1 also works
        {
            _items[entityIndex] = entity;
            SaveData();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// This implementation safely checks if the item exists before removing.
    /// </remarks>
    public void Delete(int id)
    {
        var itemToDelete = GetById(id);
        if (itemToDelete != null)
        {
            _items.Remove(itemToDelete);
            SaveData();
        }
    }
}