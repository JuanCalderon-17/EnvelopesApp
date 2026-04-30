# cdpTracker — CDP Kiosk Envelope Manager

![Angular](https://img.shields.io/badge/Angular-19-red?logo=angular&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue?logo=postgresql&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-orange?logo=jsonwebtokens&logoColor=white)

> A real-world internal tool built for McDonald's **CDP (Centro de Postres)** kiosk workers to digitize and simplify weekly envelope tracking and financial reporting.

---

## About The Project

McDonald's CDP locations are small dessert kiosks placed in malls and terminals. Each kiosk (K2, K3, K5) operates independently, and workers must record daily envelope codes and amounts to generate accurate weekly financial summaries for managers.

Before this app, this process was done manually — prone to errors and time-consuming at shift end. **cdpTracker** replaces that with a simple, authenticated web interface where each worker logs in, records their envelopes for the day (identified by a 4-digit code and amount), and can instantly review their full week grouped by day.

Managers benefit from a clean, consistent data structure that makes weekly reporting straightforward.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 19, TypeScript 5.7, Reactive Forms |
| Backend | ASP.NET Core Web API (.NET 9) |
| Authentication | JWT Bearer tokens, BCrypt password hashing |
| Database | PostgreSQL 16, Entity Framework Core 9 |
| Background Tasks | .NET Hosted Service (scheduled cleanup) |

---

## Features

- **Secure login** — JWT-based authentication with role-based claims (Worker / Manager)
- **Kiosk-scoped isolation** — workers can only access their own envelope records
- **Weekly dashboard** — envelopes grouped by day with Spanish date labels, weekly totals (count + sum)
- **Envelope validation** — 4-digit code required, unique per worker per day, amount must be positive
- **Server-side timestamps** — recording date/time is set automatically on the backend (UTC)
- **Week navigation** — browse previous and future weeks from the dashboard
- **Auto-cleanup** — a background service runs at midnight UTC and deletes records older than 3 months
- **Auth interceptor** — JWT token is automatically attached to every outgoing HTTP request

---

## Architecture

```
Angular SPA (localhost:4200)
        │
        │  HTTP + JWT Bearer
        ▼
ASP.NET Core Web API (localhost:7091)
        │
        │  Entity Framework Core
        ▼
PostgreSQL Database (cdp_tracker_db)
```

- Route `/dashboard` is protected by an **auth guard** — unauthenticated users are redirected to login
- An **HTTP interceptor** reads the token from `localStorage` and injects it into every request header
- The backend validates the JWT on every protected endpoint and checks worker ownership before returning data

---

## Getting Started

### Prerequisites

- [Node.js 20+](https://nodejs.org/) and Angular CLI 19 (`npm install -g @angular/cli`)
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16](https://www.postgresql.org/) running locally

### Backend Setup

```bash
cd cdpTracker-Api

# Apply database migrations (creates tables and seeds default workers)
dotnet ef database update

# Run the API
dotnet run
# Listening on https://localhost:7091
```

> Default database connection: `Host=localhost;Port=5432;Database=cdp_tracker_db;Username=postgres;Password=admin123`
> Update `appsettings.json` to match your local PostgreSQL credentials.

### Frontend Setup

```bash
cd cdpTracker-Frontend

npm install

npm start
# App running on http://localhost:4200
```

---

## API Reference

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| `POST` | `/api/auth/login` | No | Authenticate a worker, returns a JWT token |
| `POST` | `/api/envelopes` | Yes | Record a new envelope entry |
| `GET` | `/api/envelopes/worker/{workerId}` | Yes | Retrieve all envelopes for a worker |

---

## Seeded Users

After running migrations, two default accounts are created:

| Name | Role | Kiosk |
|---|---|---|
| Admin Manager | Manager | K2 |
| Juan Calderon | Worker | K3 |

---

## Screenshots

<!-- TODO: Add screenshots -->

| Login Screen | Weekly Dashboard |
|---|---|
| _Screenshot coming soon_ | _Screenshot coming soon_ |

---

## Author

**Juan** — [GitHub](https://github.com/JuanCalderon-17)

---

## License

This project is intended for internal use. No open-source license is applied.
