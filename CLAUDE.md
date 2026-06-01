# CLAUDE.md — Project Guide for AI Assistants

## Project Overview

Monorepo: React frontend + .NET 10 backend + Elasticsearch. User search with autocomplete.

## Directory Map

```
user-search-app/
├── frontend/          React + TypeScript + SCSS (Vite)
├── backend/
│   ├── UserSearch.Api/         Main API project
│   └── UserSearch.Api.Tests/   xUnit + Moq tests
└── docker-compose.yml
```

## Key Conventions

- **ES index name:** `users`
- **Backend port:** 5000 (local), 8080 (container internal)
- **Frontend dev port:** 5173 (Vite), 3000 (Docker/Nginx)
- **API base:** `/api/users`
- **SCSS:** one `.scss` file per component, co-located with the `.tsx`
- **Hooks:** business logic lives in `src/hooks/`, components are presentational
- **Validation:** mirrored client (`utils/validation.ts`) and server (`CreateUserRequest.cs`)

## Dev Commands

### Backend
```bash
cd backend/UserSearch.Api && dotnet run          # start API
cd backend && dotnet test                        # run unit tests
dotnet add package Elastic.Clients.Elasticsearch # add ES client
```

### Frontend
```bash
cd frontend && npm install && npm run dev        # start dev server
npm run build                                    # production build
```

### Docker
```bash
docker-compose up --build                        # full stack
docker-compose up elasticsearch                  # ES only (for backend local dev)
curl http://localhost:9200/users/_count          # verify seed data
curl "http://localhost:5000/api/users/autocomplete?q=wa"
```

## Elasticsearch Index

Custom analyzer on `firstName`, `lastName`, `fullName` fields:
- Index-time: `standard` tokenizer + `edge_ngram` (min=2, max=20) — enables prefix match on each token
- Search-time: `standard` tokenizer + `lowercase` only (no ngram on query)

This means `"wa"` matches Walker AND Walker-Smith (hyphen splits into two tokens).

## Seed Data

12 users seeded from code in `ElasticsearchSetup.cs` on startup. Seeding is idempotent — skipped if doc count > 0.

## Service Responsibilities

- `ElasticsearchUserRepository` — all ES queries (autocomplete, search, create, duplicate check)
- `UserService` — orchestration: calls repo, handles duplicate logic, maps models
- `UsersController` — HTTP layer only, delegates to `IUserService`

## Test Structure

`UserSearch.Api.Tests/Services/UserServiceTests.cs` — mocks `IUserRepository` with Moq.
Tests cover: search, autocomplete, create (happy path), duplicate email (conflict), invalid phone, invalid email.
