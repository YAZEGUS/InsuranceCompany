using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

/// <summary>
/// Контекст бази даних, який зв'язує сутності Domain з таблицями SQL.
/// Використовує SQLite для зберігання даних у файлі InsuranceDB.db
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; } // Таблиця для клієнтів
    public DbSet<Policy> Policies { get; set; } // Таблиця для страхових полісів
    public DbSet<Claim> Claims { get; set; } // Таблиця для страхових подій
    public DbSet<Agent> Agents { get; set; } // Таблиця для страхових агентів
    public DbSet<Request> Requests { get; set; } // Таблиця для запитів клієнтів
    public DbSet<Payment> Payments { get; set; } // Таблиця для обліку транзакцій (внески/виплати)

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=InsuranceDB.db");
    }
}