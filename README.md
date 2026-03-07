# Sprint 1 — Event Management Web API

Простой сервис на ASP.NET Core Web API для управления мероприятиями.

## Что реализовано

- CRUD API для событий;
- хранение данных в памяти приложения;
- бизнес-логика вынесена в сервис `EventService`;
- сервис подключён через DI;
- валидация входных данных;
- OpenAPI-документ приложения;
- Swagger UI для быстрого тестирования API.

## Требования

- .NET SDK 10.0 или выше.

## Запуск проекта

```bash
dotnet restore
dotnet build
dotnet run --project Sprint-1/Sprint-1.csproj
```

После запуска API будет доступен по адресу из `launchSettings.json`, например:

- `http://localhost:5187`
- Swagger UI: `http://localhost:5187/swagger`
- OpenAPI: `http://localhost:5187/openapi/v1.json`

## Основные эндпоинты

- `GET /events` — получить список событий;
- `GET /events/{id}` — получить событие по идентификатору;
- `POST /events` — создать событие;
- `PUT /events/{id}` — обновить событие;
- `DELETE /events/{id}` — удалить событие.

## Пример тела запроса для создания/обновления

```json
{
  "title": "Sprint Planning",
  "description": "Планирование задач на неделю",
  "startAt": "2026-03-10T10:00:00Z",
  "endAt": "2026-03-10T11:00:00Z"
}
```

## Правила валидации

- `Title` обязателен;
- `StartAt` обязателен;
- `EndAt` обязателен;
- `EndAt` должен быть позже `StartAt`.