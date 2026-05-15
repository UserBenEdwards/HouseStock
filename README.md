# Housestock

Console application for managing a household appliances warehouse.
Built with C# / .NET 8 following clean layered architecture principles.

**Course:** SE214 · **Stack:** .NET 8 · EF Core 9 · MySQL 8 · xUnit + Moq · Serilog → Seq

---

## Features

- Browse appliances by category or price range
- Full CRUD for appliances and categories (Admin mode)
- Role-based access: **USER** (read-only) and **ADMIN** (full CRUD)
- Password-protected admin mode
- Structured logging via Serilog → Seq

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for MySQL + Seq)

---

## Quick Start

**1. Start the database and Seq:**
```bash
docker compose up -d
```

**2. Run the application:**
```bash
dotnet run --project AppMain
```

The database is created and seeded automatically on first launch.

**3. Run tests:**
```bash
dotnet test
```

---

## Docker Services

| Service | Port | Description |
|---------|------|-------------|
| MySQL 8 | `3306` | Main database |
| Seq | `15341` | Log viewer UI → http://localhost:15341 |

---

## Configuration

`Housestock/AppMain/appsettings.json`:
```json
{
  "Auth": {
    "AdminPassword": "secret123"
  },
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=housestock;Uid=root;Pwd=root;"
  }
}
```

---

## Commands

### USER mode

| Command | Description |
|---------|-------------|
| `find all` | List all appliances |
| `find <category>` | Filter by category name |
| `find all price=min;max` | Filter by price range |
| `find <category> price=min;max` | Filter by category and price |
| `show <id>` | Show appliance details |
| `cost <min> <max>` | Appliances in price range |
| `categories` | List all categories |
| `help` | Show available commands |
| `switch admin` | Prompt for admin password |
| `exit` | Exit the application |

### ADMIN mode (additional)

| Command | Description |
|---------|-------------|
| `add` | Add appliance (interactive) |
| `update <id>` | Update appliance (interactive, Enter to keep) |
| `delete <id>` | Delete appliance (with confirmation) |
| `add-category` | Add category (interactive) |
| `update-category <id>` | Update category (interactive, Enter to keep) |
| `delete-category <id>` | Delete category (with confirmation) |
| `switch user` | Return to USER mode |

> Deleting a category does **not** delete its appliances — their `CategoryId` is set to `NULL`.

---

## Transaction Management

`DbSeeder.SeedAsync` wraps the entire initial data load in an **explicit transaction**:

```csharp
await using var tx = await context.Database.BeginTransactionAsync();

context.ApplianceCategories.AddRange(categories);
await context.SaveChangesAsync();   // save categories

context.Appliances.AddRange(appliances);
await context.SaveChangesAsync();   // save appliances

await tx.CommitAsync();             // commit both or rollback all
```

Without the transaction, a failure between the two `SaveChangesAsync` calls would leave categories committed but appliances missing — an inconsistent DB state.
All other write operations (Add, Update, Delete) are single `SaveChangesAsync` calls, which EF Core already wraps in an implicit transaction automatically.

---

## Architecture

```
Domain          — Entities, Interfaces, Exceptions, Specifications, Validation
Infrastructure  — EF Core DbContext, Repositories, Migrations
Service         — Business logic, DTOs, Services
AppController   — Commands, Registry, Controllers
View            — ConsolePresentation, Renderer
AppMain         — Composition Root, Program.cs, DI setup
Test            — Unit + Integration tests
```

Dependencies flow **inward only**. Domain has no external dependencies.

---

## Design Patterns

| Pattern | Where |
|---------|-------|
| **Command** | Each command (`add`, `find`, …) is a separate `ICommand` class |
| **Repository** | Service layer works through `IApplianceRepository` interface |
| **Specification** | `ByCategorySpecification`, `PriceRangeSpecification` with `.And()` / `.Or()` combinators |
| **Fluent Validation** | `Validator<T>.For(dto).NotNullOrEmpty().MaxLength().GreaterThan().Validate()` |
| **Options Pattern** | `AuthOptions` bound to `appsettings.json` via `IOptions<T>` |
| **Dependency Injection** | `Microsoft.Extensions.DI` — all dependencies via constructor |
| **Composition Root** | AppMain is the single wiring point |

---

## Exception Hierarchy

```
HousestockException
├── ValidationException      — failed validation (Errors list)
├── PersistenceException     — unexpected DB error (wraps InnerException)
├── ApplianceNotFoundException
├── CategoryNotFoundException
├── DuplicateApplianceException
└── DuplicateCategoryException
```

---

## Tests — 99 total

| File | Type | Count |
|------|------|-------|
| `ApplianceServiceTests` | Unit | 16 |
| `CategoryServiceTests` | Unit | 10 |
| `AuthServiceTests` | Unit | 6 |
| `RequestParserTests` | Unit | 14 |
| `SpecificationTests` | Unit | 22 |
| `ValidatorTests` | Unit | 15 |
| `ApplianceIntegrationTests` | Integration | 7 |
| `ControllerIntegrationTests` | Integration | 8 |

Unit tests use **Moq** for isolation. Integration tests run a full DI stack with **EF Core InMemory** — each test gets an isolated database via a unique `Guid`.
