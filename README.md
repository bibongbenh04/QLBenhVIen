# Hospital Management System

A comprehensive Hospital Management System built with ASP.NET Core MVC and SQL Server.

## Features

- Multi-role authentication (Admin, Doctor, Staff, Patient)
- Patient management
- Doctor management with specialties and schedules
- Appointment scheduling
- Medical records management
- Medication and pharmacy inventory
- Billing and insurance processing
- Reporting and analytics

## Technology Stack

- ASP.NET Core MVC
- Entity Framework Core (Code-First)
- SQL Server
- ASP.NET Identity for authentication
- Bootstrap 5 for responsive UI
- Chart.js for data visualization

## Getting Started

### Prerequisites

- Visual Studio 2022
- .NET 6.0 SDK or later
- SQL Server (LocalDB or higher)

### Setup Instructions

1. Clone the repository
2. Open the solution in Visual Studio
3. Update the connection string in `appsettings.json` to point to your SQL Server instance
4. Open Package Manager Console and run:
   \`\`\`
   Update-Database
   \`\`\`
5. Run the application (F5)

### Default Users

The system comes with pre-configured users:

| Username | Password | Role |
|----------|----------|------|
| admin@hospital.com | Admin@123 | Administrator |
| doctor1@hospital.com | Doctor@123 | Doctor |
| staff1@hospital.com | Staff@123 | Staff |
| patient1@hospital.com | Patient@123 | Patient |

## Project Structure

- **HospitalManagement.Web**: MVC Web Application
- **HospitalManagement.Data**: Data Access Layer
- **HospitalManagement.Core**: Business Logic Layer
- **HospitalManagement.Models**: Domain Models

## License

This project is licensed under the MIT License - see the LICENSE file for details.
\`\`\`
