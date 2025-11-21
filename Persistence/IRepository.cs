using Domain;

namespace Persistence;

/// <summary>
/// Визначає загальний контракт для Сховища (Repository).
/// </summary>
/// <typeparam name="T">Тип сутності, що має успадковувати BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Додає нову сутність до сховища.
    /// </summary>
    /// <param name="entity">Сутність для додавання.</param>
    void Add(T entity);

    /// <summary>
    /// Отримує всі сутності зі сховища.
    /// </summary>
    /// <returns>Список усіх сутностей.</returns>
    List<T> GetAll();

    /// <summary>
    /// Отримує сутність за її унікальним ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор сутності.</param>
    /// <returns>Знайдена сутність або null, якщо не знайдено.</returns>
    T? GetById(int id);

    /// <summary>
    /// Оновлює існуючу сутність у сховищі.
    /// </summary>
    /// <param name="entity">Сутність для оновлення.</param>
    /// <returns>True, якщо сутність знайдено та оновлено, інакше - false.</returns>
    bool Update(T entity);

    /// <summary>
    /// Видаляє сутність за її унікальним ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор сутності для видалення.</param>
    /// <returns>True, якщо сутність знайдено та видалено, інакше - false.</returns>
    bool Delete(int id);
}