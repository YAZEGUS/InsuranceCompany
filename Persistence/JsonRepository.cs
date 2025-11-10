using Domain;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace Persistence;

public class JsonRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly string _filepath;
    private List<T> _items;
    public JsonRepository(string filepath)
    {
        _filepath = filepath;
        _items = new List<T>();
        LoadData();
    }

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

    public void Add(T entity)
    { 
        int newItemId = _items.Count > 0 ? _items.Max(item => item.Id) + 1 : 1;
        entity.Id = newItemId;
        _items.Add(entity);
        SaveData();
    }

    public void Update(T entity)
    {
        int entityIndex = _items.FindIndex(item => item.Id == entity.Id);
        if (entityIndex >= 0)
        {
            _items[entityIndex] = entity;
            SaveData();
        }
    }

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