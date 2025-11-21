using Domain;

namespace BusinessLogic.Interfaces;

/// <summary>
/// Контракт для Сервісу клієнтів. Визначає бізнес-операції, пов'язані з клієнтами.
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Отримує список усіх клієнтів.
    /// </summary>
    List<Client> GetAllClients();
    
    /// <summary>
    /// Отримує єдиного клієнта за його унікальним ідентифікатором.
    /// </summary>
    Client GetClientById(int id);
    
    /// <summary>
    /// Створює нового клієнта.
    /// </summary>
    /// <param name="fullName">Повне ім'я клієнта.</param>
    /// <param name="email">Електронна пошта.</param>
    /// <param name="clientType">Тип клієнта.</param>
    /// <returns>Новостворений клієнт.</returns>
    Client CreateClient(string fullName, string email, ClientTypes clientType);

    /// <summary>
    /// Оновлює статистичні поля клієнта (кількість полісів, виплати).
    /// </summary>
    void UpdateClientStats(int clientId, int policyChange = 0, decimal payoutChange = 0m);

    /// <summary>
    /// Видаляє клієнта за його ідентифікатором.
    /// </summary>
    /// <returns>True, якщо видалено, false, якщо не знайдено або має активні поліси.</returns>
    bool DeleteClient(int clientId);
}