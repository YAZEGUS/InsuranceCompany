using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

/// <summary>
/// Реалізація IRepository<T> для роботи з базою даних через Entity Framework Core.
/// </summary>
public class SqlRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public SqlRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public void Add(T entity)
    {
        _dbSet.Add(entity);
        _context.SaveChanges();
    }

    public List<T> GetAll()
    {
        return _dbSet.ToList(); 
    }

    public T? GetById(int id)
    {
        return _dbSet.FirstOrDefault(e => e.Id == id);
    }

    public bool Update(T entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
        return true; 
    }

    public bool Delete(int id)
    { 
        var entity = GetById(id);
        if (entity == null) return false;
        
        _dbSet.Remove(entity);
        _context.SaveChanges();
        return true;
    }
}