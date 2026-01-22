# URL Shortener Service

A modern URL shortening service built with .NET 8 and Razor Pages. Create short, memorable links with click tracking and analytics.

## Features

- **Shorten URLs** - Convert long URLs into short, shareable links
- **Custom Slugs** - Create custom short codes (e.g., `/myproject`)
- **Click Analytics** - Track clicks, referrers, and geographic data
- **Link Expiration** - Set expiration dates for temporary links
- **Dashboard** - View all your links and their performance
- **Caching** - Fast redirects with in-memory caching

## Tech Stack

- **.NET 8** - Latest LTS framework
- **ASP.NET Core** - Web framework
- **Razor Pages** - Server-side rendered UI
- **Entity Framework Core** - ORM for data access
- **SQL Server / SQLite** - Database storage
- **Memory Cache** - High-performance caching

## Project Structure

```
UrlShortener/
├── src/
│   ├── UrlShortener.Domain/        # Entities, interfaces, business logic
│   ├── UrlShortener.Infrastructure/ # Data access, caching, external services
│   └── UrlShortener.Web/           # Razor Pages, API controllers
└── UrlShortener.sln
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (or use SQLite for development)

### Run the Application

```bash
cd src/UrlShortener.Web
dotnet run
```

Navigate to `https://localhost:5001`

### Database Setup

The application uses Entity Framework Core migrations:

```bash
cd src/UrlShortener.Web
dotnet ef database update
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/urls` | Create a new short URL |
| GET | `/api/urls` | Get all URLs for current user |
| GET | `/api/urls/{id}` | Get URL details with analytics |
| DELETE | `/api/urls/{id}` | Delete a short URL |
| GET | `/{shortCode}` | Redirect to original URL |

## Configuration

Configure the application in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your connection string"
  },
  "UrlShortener": {
    "BaseUrl": "https://short.app",
    "DefaultExpiration": "30.00:00:00"
  }
}
```

## License

MIT License - feel free to use this project for learning or personal projects.

## Author

**Ebenezer Doutimiwei** - [GitHub](https://github.com/Karinateii)

---

*Built with .NET 8 and a lot of coffee ☕*
