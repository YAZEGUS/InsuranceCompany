using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

/// <summary>
/// Контекст бази даних, який зв'язує сутності Domain з таблицями SQL.
/// Використовує SQLite для зберігання даних у файлі InsuranceDB.db
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Payment> Payments { get; set; } // Із 4-го етапу

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Рядок підключення. Файл бази даних (InsuranceDB.db) буде створено автоматично.
        optionsBuilder.UseSqlite("Data Source=InsuranceDB.db");
    }
}