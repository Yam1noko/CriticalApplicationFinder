# SummerProject

Fullstack-приложение на ASP.NET Core + Angular + PostgreSQL, завернутое в Docker.

## Стек

- Backend: ASP.NET Core 9, EF Core, PostgreSQL
- Frontend: Angular 17
- БД: PostgreSQL
- DevOps: Docker Compose

## Быстрый запуск

```bash
docker compose up --build -d
```

- Frontend: http://localhost:4200
- Backend API: http://localhost:5000/api/
- Swagger UI: http://localhost:5000/swagger

## Структура

```
/
├── backend/           # ASP.NET Core API
├── frontend/          # Angular SPA
├── init/              # SQL-скрипты создания и наполнения БД
├── docker-compose.yml
└── README.md
```

## Примечания

- EF Core создает таблицы при старте (EnsureCreated)
- Angular билдится в dist/frontend и хостится через nginx внутри контейнера