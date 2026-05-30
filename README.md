# User Search App

A full-stack user search application.

## Tech Stack

- **Frontend:** React 18 + TypeScript + SCSS (Vite)
- **Backend:** .NET 10 Web API (C#)
- **Search:** Elasticsearch 9 — edge_ngram analyzer for word-level prefix matching

## Features

- Autocomplete suggestions after typing 2+ characters (matches any word in first or last name)
- User detail cards showing name, job title, phone, and email
- Add new users with validation (UK phone format, email format, duplicate detection)
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
- `"dev"` matches users with "Developer" or "DevOps" job titles... wait, search is name-only
