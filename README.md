# Magentrix Desk — Customer Support Dashboard

A mini customer support dashboard for support managers: triage tickets, leave comments, and see what needs attention first thing in the morning.

Built as the Magentrix Senior Software Developer take-home assignment.

## Tech stack

| Layer | Choice |
| --- | --- |
| Backend | ASP.NET Core 8 Web API (C#) |
| Data | SQLite via Entity Framework Core (seeded sample tickets) |
| Frontend | Vue 3 SPA + Vue Router + Pinia |
| Styling | Tailwind CSS |

## Project structure

```
backend/     ASP.NET Core API (controllers → services → EF Core)
frontend/    Vue 3 SPA
```

## Setup and run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Node.js 18+ (20 recommended) and npm

### 1. Start the API

```bash
cd backend
dotnet restore
dotnet run --launch-profile http
```

API listens on **http://localhost:5080**  
Swagger UI: **http://localhost:5080/swagger**

SQLite database file `support.db` is created automatically and seeded on first run.

### 2. Start the frontend

```bash
cd frontend
npm install
npm run dev
```

App: **http://localhost:5173**

The SPA calls `http://localhost:5080/api` (see `frontend/.env.development`).

## Core features

- Ticket CRUD with status, priority, customer, assignee, created/updated timestamps
- Dashboard list with sort (created, priority, status, updated), filters, and text search
- Ticket detail view with comments and field-level history
- Delete confirmation, loading skeletons, and empty states
- Validation on both API (data annotations) and UI (required fields)

## Extras beyond the brief (and why)

1. **Morning brief** — Open queue, high-priority active, aging unassigned, and “touched today” counts. Support managers open a dashboard to decide *what to do first*; stats make that decision faster than a raw table.
2. **Needs attention focus** — One-click filter for high priority, unassigned, or tickets older than 3 days. Cuts triage time when the queue is noisy.
3. **Assignee + unassigned callouts** — Ownership is how queues actually move; unassigned aging tickets are highlighted.
4. **Ticket history audit trail** — Status/priority/assignee/title changes are logged so managers can reconstruct “what happened” without digging through comments.
5. **Quick status chips on detail** — Change status without opening the full edit form — common manager action during standups.
6. **Age-in-days** — Makes SLA pressure visible at a glance in the list.

## Assumptions and design decisions

- **Mock current user**: comments/history use a free-text author (default “Support Manager”) — no auth for this take-home.
- **Filtering/sorting on the server**: keeps list behavior consistent and ready for larger datasets.
- **SQLite + EnsureCreated**: zero-config local persistence; fine for a demo, not a migration story for production.
- **Enums as integers** over the wire (`0–3` status, `0–2` priority) with labels on the client — simple and explicit.
- **CORS** limited to the Vite dev origin.
- **Desktop-first** responsive layout as allowed by the brief.

## API overview

| Method | Path | Purpose |
| --- | --- | --- |
| GET | `/api/tickets` | List (+ `status`, `priority`, `search`, `assignee`, `sortBy`, `sortDir`, `needsAttention`) |
| GET | `/api/tickets/stats` | Dashboard summary counts |
| GET | `/api/tickets/{id}` | Detail + comments + history |
| POST | `/api/tickets` | Create |
| PUT | `/api/tickets/{id}` | Update |
| DELETE | `/api/tickets/{id}` | Delete |
| POST | `/api/tickets/{id}/comments` | Add comment |

## Architecture notes

**Backend:** Controllers stay thin; `TicketService` owns query logic, history tracking, and DTO mapping. EF Core models are isolated from API contracts.

**Frontend:** Pinia holds list/detail/filter state; views compose presentational components (`TicketTable`, badges, modals). Router separates dashboard vs detail.

## What I’d improve with more time

- Auth (roles: agent vs manager), real assignee directory
- Pagination + full-text search indexes
- Automated tests (API integration + Vue component tests)
- Soft delete, attachments, email notifications / SLA timers
- EF migrations and a production-ready hosting story (Docker Compose)

## License / confidentiality

Prepared for Magentrix Corporation candidate evaluation. Confidential — for candidate use only per assignment terms.
