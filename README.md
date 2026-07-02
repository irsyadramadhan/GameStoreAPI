# 🎮 GameStore API

A RESTful API for a game store built with ASP.NET Core 8, featuring authentication, product catalog, shopping cart, order management, and game reviews.

## 🔗 Links

- **Live API:** `https://your-app.up.railway.app/swagger`
- **GitHub:** `https://github.com/username/GameStoreAPI`

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer + Refresh Token |
| Documentation | Swagger UI (Swashbuckle) |
| Deployment | Railway (Docker) |

---

## ✨ Features

- **JWT Authentication** — Register, login, and refresh token with role-based access control (Admin/User)
- **Game Catalog** — Browse games with search, filter by category, price range, and pagination
- **Categories** — Manage game categories (Admin only)
- **Shopping Cart** — Add and remove games from cart (per user)
- **Orders** — Checkout from cart with transaction handling; order history and detail
- **Reviews** — Leave a review only for purchased games; one review per game per user
- **Soft Delete** — Deleted games are hidden from catalog but preserved in order history

---

## 📐 Architecture

```
Request → Controller → Service → EF Core (DbContext) → PostgreSQL
```

- **Controllers** — Handle HTTP requests and responses
- **Services** — Contain business logic and validation
- **EF Core** — Data access layer with code-first migrations
- **DTOs** — Separate API contracts from internal models

---

## 🗂️ Project Structure

```
GameStoreAPI/
├── Controllers/        # HTTP endpoints
├── Services/           # Business logic (interface + implementation)
├── Models/             # Entity classes
├── DTOs/               # Request & Response models
│   ├── Auth/
│   ├── Game/
│   ├── Cart/
│   ├── Order/
│   └── Review/
├── Data/               # AppDbContext & migrations
├── Helpers/            # JWT generator, claims extensions
└── Program.cs          # DI container & middleware pipeline
```

---

## 📋 API Endpoints

### Auth
| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register new user |
| POST | `/api/auth/login` | Public | Login and get tokens |
| POST | `/api/auth/refresh` | Public | Refresh access token |

### Games
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/games` | Public | List games (search, filter, pagination) |
| GET | `/api/games/{id}` | Public | Get game detail |
| POST | `/api/games` | Admin | Create game |
| PUT | `/api/games/{id}` | Admin | Update game |
| DELETE | `/api/games/{id}` | Admin | Soft delete game |

### Categories
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/categories` | Public | List all categories |
| POST | `/api/categories` | Admin | Create category |

### Cart
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/cart` | User | View cart |
| POST | `/api/cart/items` | User | Add game to cart |
| DELETE | `/api/cart/items/{id}` | User | Remove item from cart |

### Orders
| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/orders/checkout` | User | Checkout from cart |
| GET | `/api/orders` | User | Order history |
| GET | `/api/orders/{id}` | User | Order detail |

### Reviews
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/games/{id}/reviews` | Public | Get reviews for a game |
| POST | `/api/games/{id}/reviews` | User | Review a purchased game |

---

## 🚀 Run Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)

### Steps

1. Clone the repository
```bash
git clone https://github.com/username/GameStoreAPI.git
cd GameStoreAPI
```

2. Create `appsettings.json` from the example file
```bash
cp appsettings.Example.json appsettings.json
```

3. Fill in your local config in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gamestoredb;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-characters",
    "Issuer": "GameStoreAPI",
    "Audience": "GameStoreClient",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

4. Run migrations
```bash
dotnet ef database update
```

5. Run the app
```bash
dotnet run
```

6. Open Swagger UI at `https://localhost:{port}/swagger`

---

## 🔐 Testing Auth in Swagger

1. Register via `POST /api/auth/register`
2. Login via `POST /api/auth/login` — copy the `accessToken`
3. Click **Authorize** button (top right)
4. Enter `Bearer {your_token}` and click Authorize
5. All protected endpoints are now accessible
