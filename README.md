# EducationCenter API

## Стек

- .NET 8, ASP.NET Core Web API (контроллеры, Dependency Injection, async/await)
- EF Core 8 — InMemory-хранилище + сидирование демо-данных
- FluentValidation — валидация входных DTO
- Swashbuckle — Swagger UI
- xUnit, Moq, FluentAssertions — юнит-тесты

## Бизнес-правила скидок

Скидки не суммируются: для каждого курса применяется одна — максимальная из доступных.

| Скидка | Условие |
|---|---|
| «Горящие группы», −20% | До старта курса менее 5 дней; курс, который уже начался, также считается горящим |
| «Выпускник», −30% | У пользователя `IsGraduate = true` |

## API

| Метод | Маршрут | Описание |
|---|---|---|
| GET | `/api/courses` | Список курсов (демо-данные) |
| GET | `/api/users` | Список пользователей (демо-данные) |
| POST | `/api/cart/calculate` | Расчёт стоимости корзины со скидками |
| POST | `/api/leads` | Заявка «Обратный звонок» |

### POST /api/cart/calculate

Id пользователя и курсов берутся из `GET /api/users` и `GET /api/courses`.

```json
{
  "userId": "<guid пользователя>",
  "courseIds": ["<guid курса 1>", "<guid курса 2>"]
}
```

Ответ `200 OK` (выпускник, оба курса со скидкой 30% — она больше «горящих» 20%):

```json
{
  "items": [
    {
      "courseId": "…",
      "name": "C#: основы программирования",
      "basePrice": 24900,
      "appliedDiscountPercent": 0.30,
      "finalPrice": 17430
    },
    {
      "courseId": "…",
      "name": "SQL и базы данных",
      "basePrice": 19900,
      "appliedDiscountPercent": 0.30,
      "finalPrice": 13930
    }
  ],
  "totalOriginal": 44800,
  "totalFinal": 31360
}
```

### POST /api/leads

Тип клиента передаётся строкой: `Individual` (физ. лицо) или `LegalEntity` (юр. лицо).

```json
{
  "name": "Иван Иванов",
  "phone": "+7 (999) 123-45-67",
  "email": "ivan@example.com",
  "clientType": "LegalEntity"
}
```

Ответ `200 OK` — всегда успешный для клиента:

```json
{
  "success": true,
  "message": "Заявка принята. Мы свяжемся с вами в ближайшее время."
}
```

## Структура проекта

```text
EducationCenter.slnx
├── EducationCenter/            # ASP.NET Core Web API
│   ├── Contracts/              # DTO: запросы и ответы API (Carts, Leads)
│   ├── Controllers/            # CartController, LeadsController, CoursesController, UsersController
│   ├── Domain/                 # Сущности Course, User, enum ClientType, EF DbContext, DbSeeder
│   ├── Infrastructure/Email/   # IEmailSender, SMTP-отправитель, демо-заглушка с имитацией сбоя
│   ├── Middlware/              # GlobalExceptionHandler
│   ├── Services/
│   │   ├── Discounts/          # IDiscountPolicy, политики скидок, CartPriceCalculator
│   │   └── Leads/              # LeadService — fallback-логика отправки заявок
│   └── Validation/             # FluentValidation-валидаторы
└── EducationCenter.Tests/      # xUnit: CartPriceCalculatorTests, LeadServiceTests
```
