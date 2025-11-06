Структура рішення складається з таких проєктів:
### 1. `InsuranceCompany.Domain`

* **Призначення:** Містить лише "чисті" класи сутностей.

### 2. `InsuranceCompany.Persistence`

* **Призначення:** Рівень доступу до даних (Persistence Layer). Відповідає тільки за збереження та читання даних.

### 3. `InsuranceCompany.BusinessLogic` (Бібліотека Класів)

* **Призначення:** Рівень бізнес-логіки (Business Logic Layer). Тут знаходиться логіка програми.

### 4. `InsuranceCompany.ConsoleUI` (Консольний Додаток)

* **Призначення:** Рівень презентації (Presentation Layer).
