# Syndication Tools

A prototype data aggregation service that collects, caches, and serves content from syndication sources. The service currently supports RSS feeds and is designed with an extensible architecture to accommodate additional source types.

## Architecture

The solution follows a layered architecture:

```
Controllers  →  Services  →  Domain Models  →  Entity Framework  →  SQL Server
```

**Design patterns and approaches applied:**

- **Generic service base class** with reusable CRUD operations across all entity types
- **Factory-based dependency injection** with per-request scoping (custom `IDependencyResolver`)
- **Transaction wrapper** (`ITransaction`) for atomic multi-step operations
- **Polymorphic JSON serialization** via a custom `JsonConverter` supporting the `Source` type hierarchy
- **Table-per-type (TPT) inheritance mapping** in Entity Framework for `Source` and `Content` hierarchies
- **OWIN middleware pipeline** for composable request processing (CORS, OAuth, Web API)
- **Centralized exception logging** through a custom `IExceptionLogger` implementation

## Solution Structure

| Project | Description |
|---|---|
| **AggregationService** | ASP.NET Web API 2 REST service hosted on OWIN. Implements feed aggregation, HTTP-compliant caching, OAuth 2.0 authentication, and content revalidation. |
| **AggregationService.Tests** | Unit and integration test suite covering controllers, services, utilities, and full HTTP pipeline scenarios. |
| **AggregationServiceClient** | Console application that consumes the AggregationService API through an interactive menu-driven interface. |

## Technologies

| Category | Technology | Version |
|---|---|---|
| Runtime | .NET Framework | 4.8 |
| Web Framework | ASP.NET Web API | 5.2.4 |
| Hosting | OWIN (Microsoft.Owin) | 4.2.2 |
| ORM | Entity Framework | 6.5.1 |
| Database | SQL Server LocalDB | — |
| Authentication | OAuth 2.0 (Bearer Tokens) | — |
| Logging | NLog | 5.4.0 |
| Serialization | Newtonsoft.Json | 13.0.4 |
| Testing | NUnit | 3.14.0 |
| Mocking | Moq | 4.18.4 |
| Integration Testing | Microsoft.Owin.Testing | 4.2.2 |

## Key Features

- **HTTP cache compliance** — respects `Cache-Control` (max-age), `Expires`, and `Last-Modified` headers from upstream sources
- **Content revalidation** — uses `If-Modified-Since` conditional requests and handles `304 Not Modified` responses to minimize bandwidth
- **Atomic operations** — source creation with initial content fetch is wrapped in an explicit database transaction
- **Extensible source types** — the `Source` → `Feed` → `RssFeed` hierarchy and pluggable `Parser`/`Reader` design allow new syndication formats to be added with minimal changes
- **OAuth 2.0 security** — all API endpoints require bearer token authentication via the Resource Owner Password Credentials flow

## API Endpoints

All endpoints (except token acquisition) require a valid bearer token in the `Authorization` header.

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/auth/token` | Obtain an OAuth 2.0 bearer token |
| `GET` | `/api/collections` | List all collections with their sources |
| `POST` | `/api/collections` | Create a new collection |
| `POST` | `/api/sources` | Add a source to a collection (fetches initial content) |
| `GET` | `/api/supportedsourcetypes` | List supported source types |
| `GET` | `/api/contents/bycollection/{id}` | Retrieve aggregated content for a collection |

## Testing

The test suite is built on **NUnit** and **Moq** and covers two levels:

- **Unit tests** — validate controllers, services, and utilities in isolation using mocked `DbSet` and service dependencies
- **Integration tests** — spin up an in-memory OWIN test server (`Microsoft.Owin.Testing`) to exercise the full HTTP pipeline, including authentication, CORS, and end-to-end request handling
