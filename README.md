# SchoolAPI

A RESTful API built with **ASP.NET Core 8.0** for managing school operations including students, teachers, subjects, enrollment, grades, and attendance.

## Technology Stack

- **Language**: C#
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server (Entity Framework Core 9)
- **API Documentation**: Swagger / OpenAPI
- **Validation**: FluentValidation
- **Architecture**: Clean Architecture

## Project Structure

```
SchoolAPI/
├── School/           # API entry point – controllers, Program.cs, configuration
├── Application/      # Business logic – services, DTOs, interfaces
├── Domain/           # Core entities and repository interfaces
└── Infrastructure/   # EF Core DbContext, repositories, and migrations
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (default instance: `SQLEXPRESS`)

## Getting Started

1. **Clone the repository**

   ```bash
   git clone https://github.com/jhanc27/SchoolAPI.git
   cd SchoolAPI
   ```

2. **Configure the database connection**

   Edit `School/appsettings.Development.json` and update the `DefaultConnection` string to match your SQL Server instance.

3. **Apply database migrations**

   ```bash
   dotnet ef database update --project Infrastructure --startup-project School
   ```

4. **Run the API**

   ```bash
   dotnet run --project School
   ```

5. **Open Swagger UI** (Development mode)

   Navigate to `https://localhost:<port>/swagger` in your browser.

## API Endpoints

| Resource | Base Route | Description |
|---|---|---|
| Students | `/api/estudiantes` | CRUD + filtering and pagination |
| Teachers | `/api/profesores` | CRUD |
| Subjects | `/api/materias` | CRUD |
| Periods | `/api/periodos` | CRUD |
| Enrollments | `/api/inscripciones` | CRUD |
| Grades | `/api/calificaciones` | CRUD |
| Attendance | `/api/asistencias` | CRUD |
| Reports | `/api/reportes` | Analytics and summary reports |

### Report Endpoints

| Endpoint | Description |
|---|---|
| `GET /api/reportes/boletin-estudiante/{estudianteId}/{periodoId}` | Student report card |
| `GET /api/reportes/resumen-curso/{periodoId}` | Course summary |
| `GET /api/reportes/asistencia-diaria/{fecha}` | Daily attendance |
| `GET /api/reportes/rendimiento-estudiante/{estudianteId}` | Student performance |
| `GET /api/reportes/estadisticas-generales` | Overall statistics |

## Configuration

Key settings in `appsettings.json` / `appsettings.Development.json`:

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `OrigenesPermitidos` | Comma-separated list of allowed CORS origins (default: `http://localhost:4200`) |

## Build

```bash
dotnet build School.sln
```
