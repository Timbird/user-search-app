# User Search App

A full-stack user search application.

## Tech Stack

- **Frontend:** React 18 + TypeScript + SCSS (Vite)
- **Backend:** .NET 10 Web API (C#)
- **Search:** Elasticsearch 9 — edge_ngram analyzer for word-level prefix matching

## Features

- Autocomplete suggestions after typing 2+ characters (matches any word in first or last name)
- User detail cards showing name, job title, phone, and email
- Add new users with validation (UK phone format, email format, duplicate by email detection)
- "New user added!" success notification

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (for local dev only)
- [Node 20+](https://nodejs.org/) (for local dev only)

## Running with Docker Compose (recommended)

```bash
docker-compose up --build
```

- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- Elasticsearch: http://localhost:9200

## Local Development

### Backend

```bash
cd backend/UserSearch.Api
dotnet run
# API runs on http://localhost:5000
```

### Frontend

```bash
cd frontend
npm install
npm run dev
# App runs on http://localhost:5173
```

### Tests

The backend has two test projects with different scopes:

#### Unit Tests (`UserSearch.Api.Tests`)

Tests `UserService` in isolation using Moq to stub `IUserRepository`. No infrastructure required — runs instantly.

```bash
cd backend
dotnet test UserSearch.Api.Tests
```

Covers: autocomplete delegation, search delegation, user creation (happy path), duplicate email rejection, email normalisation, field whitespace trimming.

#### Integration Tests (`UserSearch.Api.IntegrationTests`)

Spins up a real Elasticsearch 9 container via [Testcontainers](https://dotnet.testcontainers.org/) and boots the full ASP.NET Core app against it. Tests make actual HTTP calls to the controllers and assert against live Elasticsearch responses.

**Requires Docker Desktop to be running.**

```bash
cd backend
dotnet test UserSearch.Api.IntegrationTests
```

The first run pulls the Elasticsearch Docker image (~800 MB) and will take a few minutes. Subsequent runs use the cached image and start in ~20 seconds.

Covers: autocomplete (short query, prefix match, no match), search (empty query, matching name, no match), create (valid user → 201, duplicate email → 409, invalid phone → 400, missing fields → 400).

#### Run all tests

```bash
cd backend
dotnet test
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/autocomplete?q=` | Name suggestions (≥2 chars) |
| GET | `/api/users/search?q=` | Full user search results |
| POST | `/api/users` | Create a new user |

## Search Examples

- `"wa"` matches Walker, Walker-Smith
- `"phi"` matches Phil, Phillipa