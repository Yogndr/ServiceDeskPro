# ServiceDeskPro

ServiceDeskPro is a full-stack IT service desk and incident management application built with **ASP.NET Core, Angular, Entity Framework Core, and SQL Server**.

The application provides role-based workflows for employees, administrators, and support engineers to report, assign, track, and resolve IT incidents.

## Features

### Employee
- Secure registration and login
- Create IT incidents with category and priority
- Track submitted incidents
- View incident status and SLA deadline
- View assigned support engineer
- Access detailed incident information

### Administrator
- Centralized incident management dashboard
- View incident statistics and SLA breaches
- Monitor all submitted incidents
- Dynamically load available support engineers
- Assign incidents to engineers
- Track incident ownership and progress
- Filter and monitor service requests

### Support Engineer
- View assigned incidents
- Start work on assigned requests
- Update incidents to In Progress
- Resolve completed incidents
- Track SLA information
- View complete incident details

## Incident Workflow

```text
Employee creates incident
        |
        v
       Open
        |
        v
Admin assigns engineer
        |
        v
     Assigned
        |
        v
Engineer starts work
        |
        v
   In Progress
        |
        v
Engineer resolves incident
        |
        v
     Resolved
```

## Tech Stack

### Backend
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Role-Based Authorization
- Swagger / OpenAPI

### Frontend
- Angular
- TypeScript
- HTML5
- CSS3
- Angular Signals
- Angular Router
- HttpClient

### Tools
- Visual Studio / VS Code
- SQL Server Management Studio
- Git & GitHub
- Swagger

## Core Backend Capabilities

- RESTful API architecture
- JWT-based authentication
- Role-based access control for Employee, Engineer, and Admin
- Password hashing
- Entity Framework Core database integration
- Database migrations
- Incident assignment workflow
- Incident status management
- SLA deadline calculation and breach tracking
- Dashboard statistics
- CORS configuration for Angular client

## Project Structure

```text
ServiceDeskPro/
|
|-- ServiceDeskPro.API/
|   |-- Controllers/
|   |-- Data/
|   |-- DTOs/
|   |-- Migrations/
|   |-- Models/
|   |-- Services/
|   |-- Program.cs
|   `-- appsettings.json
|
|-- servicedeskpro-client/
|   |-- src/
|   |   `-- app/
|   |       |-- core/
|   |       |   |-- guards/
|   |       |   |-- interceptors/
|   |       |   `-- services/
|   |       |
|   |       `-- pages/
|   |           |-- login/
|   |           |-- employee-dashboard/
|   |           |-- admin-dashboard/
|   |           |-- engineer-dashboard/
|   |           `-- incident-details/
|   |
|   |-- angular.json
|   `-- package.json
|
`-- README.md
```

## Security

ServiceDeskPro uses JWT Bearer authentication to secure protected API endpoints.

Authorization is role-based:

| Role | Access |
|---|---|
| Employee | Create and track personal incidents |
| Admin | Monitor incidents and assign engineers |
| Engineer | Manage assigned incidents and update status |

The JWT signing key is not stored in the repository. Local development uses **.NET User Secrets** for sensitive configuration.

## SLA Management

SLA deadlines are calculated based on incident priority.

The application tracks whether an unresolved incident has exceeded its SLA deadline and surfaces breached incidents on the administrative dashboard.

## API Documentation

When running in the development environment, Swagger provides interactive API documentation and testing.

After starting the backend, open:

```text
http://localhost:5078/swagger
```

Protected endpoints require a valid JWT token.

## Local Setup

### Prerequisites

Install:

- .NET SDK
- Node.js and npm
- Angular CLI
- SQL Server
- SQL Server Management Studio

### 1. Clone the repository

```bash
git clone <repository-url>
cd ServiceDeskPro
```

### 2. Configure the database

The default local connection uses SQL Server with Windows Authentication:

```text
Server=localhost;Database=ServiceDeskProDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Update the connection string in `ServiceDeskPro.API/appsettings.json` if your SQL Server configuration is different.

### 3. Configure the JWT secret

From the API directory:

```bash
cd ServiceDeskPro.API
dotnet user-secrets set "Jwt:Key" "your-secure-development-key"
```

### 4. Apply database migrations

```bash
dotnet ef database update
```

### 5. Start the backend

```bash
dotnet run
```

The API runs locally at:

```text
http://localhost:5078
```

### 6. Install frontend dependencies

Open another terminal:

```bash
cd servicedeskpro-client
npm install
```

### 7. Start Angular

```bash
ng serve
```

Open:

```text
http://localhost:4200
```

## Key Learning Areas

This project demonstrates hands-on implementation of:

- Full-stack application development
- REST API design
- ASP.NET Core architecture
- Angular frontend development
- SQL Server relational data modeling
- Entity Framework Core
- Authentication and authorization
- JWT token handling
- Role-based workflows
- API integration
- SLA-based business logic
- Error handling
- Git-based source control

## Future Improvements

Potential extensions include:

- Email notifications
- Incident comments and activity history enhancements
- Engineer workload-based assignment
- Search and advanced filtering
- Pagination
- File attachments
- Refresh-token authentication
- Automated unit and integration testing
- Docker-based deployment
- Cloud deployment

## Author

**Yogender**

Software Engineer | Full Stack Developer