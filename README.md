# Appointment Booking System - Interview Project

A full-stack appointment booking system built with .NET 8 and Angular 17. This project is designed for testing evaluation (unit, integration, and E2E with Playwright).

**IMPORTANT: This repository contains NO tests. Your task is to add comprehensive tests.**

## System Overview

This is a working appointment booking system with:
- **Backend**: .NET 8 ASP.NET Core Web API with clean architecture
- **Frontend**: Angular 17 with standalone components
- **Database**: SQLite with Entity Framework Core
- **API Documentation**: Swagger/OpenAPI

### Features

- Create and list healthcare providers
- Book appointments with validation
- Cancel appointments
- View provider schedules by date
- Consistent error handling with ProblemDetails

## Architecture

### Backend Structure
```
src/
├── App.Api/              # Web API controllers, startup configuration
├── App.Core/             # Domain entities, interfaces, business logic
└── App.Infrastructure/   # Data access, repositories, EF Core
```

### Frontend Structure
```
web/App.Web/
├── src/app/
│   ├── components/       # Angular components (providers, book-appointment, schedule)
│   └── services/         # ApiService for HTTP calls
```

### Key Design Decisions for Testability

1. **IClock Interface**: Time is abstracted via `IClock` interface - no direct `DateTime.UtcNow` calls in business logic
2. **Service Layer**: Business logic is in `AppointmentService` and `ProviderService`, not controllers
3. **Repository Pattern**: Data access is abstracted behind `IProviderRepository` and `IAppointmentRepository`
4. **API Service**: Frontend centralizes all HTTP calls in `ApiService`
5. **data-testid Attributes**: UI elements have stable test identifiers for E2E testing

## Business Rules

The system enforces these rules (see `AppointmentService.cs`):
- Appointment duration must be 15-120 minutes
- Appointments must start at least 30 minutes in the future
- Providers must be active to accept bookings
- No overlapping appointments (touching endpoints are allowed)
- Cancelled appointments cannot be cancelled again

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (includes npm)
- Git

### Backend Setup

1. **Navigate to the API project**:
   ```bash
   cd src/App.Api
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run database migrations** (creates SQLite database):
   ```bash
   dotnet ef database update
   ```

4. **Run the API**:
   ```bash
   dotnet run
   ```

   The API will start at `http://localhost:5000`

5. **Open Swagger UI** in your browser:
   ```
   http://localhost:5000/swagger
   ```

6. **Seed sample data** (optional, Development only):
   ```bash
   curl -X POST http://localhost:5000/api/seed
   ```

   Or use Swagger UI to call `POST /api/seed`

### Frontend Setup

1. **Navigate to the web project**:
   ```bash
   cd web/App.Web
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Start the development server**:
   ```bash
   npm start
   ```

   The Angular app will start at `http://localhost:4200`

4. **Open the app** in your browser:
   ```
   http://localhost:4200
   ```

### Quick Start (Both Backend and Frontend)

**Terminal 1 - Backend**:
```bash
cd src/App.Api
dotnet restore
dotnet ef database update
dotnet run
```

**Terminal 2 - Frontend**:
```bash
cd web/App.Web
npm install
npm start
```

**Terminal 3 - Seed Data** (optional):
```bash
curl -X POST http://localhost:5000/api/seed
```

## API Endpoints

All endpoints use `/api` prefix:

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/providers` | Create a new provider |
| GET | `/api/providers` | List all active providers |
| GET | `/api/providers/{id}/schedule?date=YYYY-MM-DD` | Get provider's appointments for a date |
| POST | `/api/appointments` | Book an appointment |
| POST | `/api/appointments/{id}/cancel` | Cancel an appointment |
| GET | `/api/now` | Get current server time (UTC) |
| POST | `/api/seed` | Seed sample data (Development only) |

### Example API Calls

**Create Provider**:
```bash
curl -X POST http://localhost:5000/api/providers \
  -H "Content-Type: application/json" \
  -d '{"name":"Dr. Smith","timeZone":"UTC"}'
```

**Book Appointment**:
```bash
curl -X POST http://localhost:5000/api/appointments \
  -H "Content-Type: application/json" \
  -d '{
    "providerId":"<guid>",
    "customerName":"John Doe",
    "startUtc":"2026-01-20T10:00:00Z",
    "endUtc":"2026-01-20T10:30:00Z"
  }'
```

## Error Handling

The API uses consistent ProblemDetails for errors:

- **400 Bad Request**: Validation errors (e.g., "Appointment must be at least 30 minutes in the future")
- **404 Not Found**: Resource not found (e.g., provider doesn't exist)
- **409 Conflict**: Business rule violations (e.g., overlapping appointments, already cancelled)

Example error response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "BadRequest",
  "status": 400,
  "detail": "Appointment must be at least 30 minutes in the future"
}
```

## Database

- **Type**: SQLite (file-based)
- **Location**: `src/App.Api/appointments.db`
- **Migrations**: Located in `App.Infrastructure`

### Reset Database

To reset the database, simply delete the file and re-run migrations:
```bash
cd src/App.Api
rm appointments.db
dotnet ef database update
```

## Testing Scope (Your Task)

This repository has **NO TESTS**. You should add:

### Backend Tests
- **Unit Tests**: Test business logic in `AppointmentService` and `ProviderService`
  - Test validation rules
  - Test overlap detection
  - Mock `IClock` to control time
  - Mock repositories
- **Integration Tests**: Test API endpoints with a test database
  - Test full request/response cycle
  - Test error handling and status codes

### Frontend Tests
- **Unit Tests**: Test components and services
  - Test `ApiService` methods
  - Test component logic
  - Mock HTTP calls
- **E2E Tests** (Playwright): Test user workflows
  - Create a provider
  - Book an appointment
  - View schedule and cancel appointment
  - Verify error messages are displayed

### Test Data Identifiers

The UI includes `data-testid` attributes for E2E testing:
- `create-provider-form`, `provider-name-input`, `create-provider-button`
- `book-appointment-form`, `appointment-provider-select`, `book-appointment-button`
- `schedule-form`, `appointment-list`, `cancel-button-{id}`
- Error and success messages have testids for assertions

## Development Notes

- **CORS**: Enabled for `http://localhost:4200` in backend
- **Proxy**: Frontend uses proxy.conf.json to route `/api` to backend
- **Time**: All times are stored and transmitted in UTC
- **No Authentication**: This is intentionally kept simple for the interview

## Technology Stack

- **.NET 8**: Latest LTS version of .NET
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core 8**: ORM with SQLite provider
- **Angular 17**: Frontend framework with standalone components
- **RxJS**: Reactive programming for async operations
- **Swagger/OpenAPI**: API documentation

## Troubleshooting

### Backend Issues

**Port already in use**:
- Change the port in `src/App.Api/Properties/launchSettings.json`
- Update the proxy config in `web/App.Web/src/proxy.conf.json`

**Database migration fails**:
- Ensure you're in the `src/App.Api` directory
- Delete `appointments.db` and try again

### Frontend Issues

**npm install fails**:
- Clear npm cache: `npm cache clean --force`
- Delete `node_modules` and `package-lock.json`, then reinstall

**CORS errors**:
- Ensure backend is running on `http://localhost:5000`
- Check browser console for details

## Project Structure

```
TestAutomationInterview/
├── src/
│   ├── App.Api/              # Web API project
│   │   ├── Controllers/      # API controllers
│   │   ├── Models/           # DTOs
│   │   ├── Program.cs        # Startup configuration
│   │   └── appsettings.json  # Configuration
│   ├── App.Core/             # Domain layer
│   │   ├── Entities/         # Domain models
│   │   ├── Interfaces/       # Abstractions
│   │   └── Services/         # Business logic
│   └── App.Infrastructure/   # Data access layer
│       ├── Data/             # DbContext
│       ├── Repositories/     # Repository implementations
│       └── Services/         # Infrastructure services
├── web/
│   └── App.Web/              # Angular project
│       ├── src/
│       │   ├── app/
│       │   │   ├── components/
│       │   │   ├── services/
│       │   │   └── app.config.ts
│       │   └── proxy.conf.json
│       ├── angular.json
│       └── package.json
├── AppointmentBooking.sln    # Solution file
└── README.md
```

## License

This is an interview project - use as needed for evaluation purposes.
