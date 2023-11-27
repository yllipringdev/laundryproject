# Backend - .NET 6

Welcome to the .NET 6 backend of our project!

## Overview

This folder contains all the server-side code and logic that powers our application. The backend is built using .NET 6, and it interacts with a SQL database to manage data.

## Prerequisites

Ensure you have the following installed before setting up the backend:

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Getting Started

1. **Clone the Repository:**
   ```bash
   git clone <repository-url>
   cd back

2. **Install Dependencies:**

 dotnet restore

3. **Database Setup:**

Create a SQL database and update the connection string in the appsettings.json file.

4. **Apply Database Migrations:**
dotnet ef database update

5. **Run the application:**
dotnet run
