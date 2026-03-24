# Event Management Web API

Сервис на ASP.NET Core Web API для управления мероприятиями.

## Что реализовано

### Sprint 1
- CRUD API для событий;
- хранение данных в памяти приложения;
- бизнес-логика вынесена в сервис `EventService`;
- сервис подключён через DI;
- валидация входных данных;
- OpenAPI-документ и Swagger UI.

### Sprint 2
- глобальная обработка ошибок через middleware (Problem Details RFC 7807);
- фильтрация событий по названию и диапазону дат;
- пагинация результатов;
- юнит-тесты на xUnit (19 тестов).

## Требования

- .NET SDK 10.0 или выше.

## Запуск проекта

```bash
dotnet restore
dotnet build
dotnet run --project Sprint-1/Sprint-1.csproj
```

После запуска:
- API: `http://localhost:5187`
- Swagger UI: `http://localhost:5187/swagger`

## Запуск тестов

```bash
dotnet test
```

## Эндпоинты

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/events` | Список событий (с фильтрацией и пагинацией) |
| GET | `/events/{id}` | Получить событие по ID |
| POST | `/events` | Создать событие |
| PUT | `/events/{id}` | Обновить событие |
| DELETE | `/events/{id}` | Удалить событие |

## Фильтрация и пагинация

`GET /events` поддерживает query-параметры:

| Параметр | Тип | По умолчанию | Описание |
|----------|-----|-------------|----------|
| `title` | string | — | Поиск по названию (регистронезависимый) |
| `from` | DateTime | — | Начало диапазона (`StartAt >= from`) |
| `to` | DateTime | — | Конец диапазона (`EndAt <= to`) |
| `page` | int | 1 | Номер страницы |
| `pageSize` | int | 10 | Размер страницы (макс. 100) |

### Примеры запросов

```
GET /events?title=sprint
GET /events?from=2026-03-01T00:00:00Z&to=2026-03-31T23:59:59Z
GET /events?title=planning&page=1&pageSize=5
```

### Пример ответа с пагинацией

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Sprint Planning",
      "description": "Планирование задач",
      "startAt": "2026-03-10T10:00:00Z",
      "endAt": "2026-03-10T11:00:00Z"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10
}
```

## Пример тела запроса (POST / PUT)

```json
{
  "title": "Sprint Planning",
  "description": "Планирование задач на неделю",
  "startAt": "2026-03-10T10:00:00Z",
  "endAt": "2026-03-10T11:00:00Z"
}
```

## Обработка ошибок

Все ошибки возвращаются в формате Problem Details (RFC 7807):

### 404 Not Found

```json
{
  "status": 404,
  "title": "Resource not found",
  "detail": "Event with id '3fa85f64-...' was not found.",
  "instance": "/events/3fa85f64-..."
}
```

### 400 Bad Request

```json
{
  "status": 400,
  "title": "Validation error",
  "detail": "EndAt must be later than StartAt.",
  "instance": "/events"
}
```

### 500 Internal Server Error

```json
{
  "status": 500,
  "title": "Internal server error",
  "detail": "...",
  "instance": "/events"
}
```

## Правила валидации

- `Title` обязателен;
- `StartAt` обязателен;
- `EndAt` обязателен;
- `EndAt` должен быть позже `StartAt`.