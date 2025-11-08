# SecureService - Home Services Application

**Course:** Secure Software Engineering (COMP SCI 7412/4812)

**Institution:** University of Adelaide

---

## 1. Project Overview

**SecureService** is a secure web application developed as part of the Secure Software Engineering group project. It provides a platform for users to find and connect with trusted home service professionals, with a strong emphasis on security, privacy, and user trust. The application is built on ASP.NET Core 8.0 and implements a wide range of security best practices.

### Key Features

- ✅ **User Authentication:** Secure user registration and login with BCrypt password hashing.

- ✅ **Provider Profiles:** Create, view, and manage service provider profiles.

- ✅ **Customer Reviews:** Users can submit reviews and ratings for providers.

- ✅ **Secure File Uploads:** Securely upload images for reviews with validation.

- ✅ **Role-Based Access Control:** Differentiates between regular users and administrators.

- ✅ **Modern UI:** A responsive, Apple-inspired frontend design.

---

## 2. How to Run on macOS (Intel & Apple Silicon)

This guide provides complete instructions for cloning, setting up, and running the project on a macOS machine.

### Prerequisites

Before you begin, ensure you have the following installed:

1. **macOS 11 (Big Sur) or later**

1. **Homebrew:** The missing package manager for macOS. [Install here](https://brew.sh/).

1. **.NET 8.0 SDK:** The software development kit for building the application.

1. **Docker Desktop for Mac:** To run SQL Server in a container. [Install here](https://www.docker.com/products/docker-desktop/).

### Step 1: Install Prerequisites

Open your Terminal and run the following commands:

```bash
# Install Homebrew (if you don't have it)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install .NET 8.0 SDK
brew install --cask dotnet-sdk

# Install Docker Desktop (or download from the link above)
brew install --cask docker
```

After installing, **launch Docker Desktop** from your Applications folder and complete its initial setup.

### Step 2: Clone the Repository

Clone the project from GitHub into a directory of your choice.

```bash
git clone https://github.com/TufanKesertoFun/SecureSoftwareEngineeringGroupProj.git
cd SecureSoftwareEngineeringGroupProj
```

### Step 3: Set Up and Run SQL Server via Docker

SQL Server does not run natively on macOS, so we use Docker. This works seamlessly on both Intel and Apple Silicon (M1/M2/M3) Macs via Rosetta 2 emulation.

```bash
# Stop and remove any old container named "sqlserver" to avoid conflicts
docker rm -f sqlserver 2>/dev/null || true

# Run the SQL Server 2022 container
docker run -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=Replace_StrongPass1! -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# Wait for 20-30 seconds for SQL Server to fully initialize
echo "Waiting for SQL Server to start..."
sleep 25

# Verify the container is running
docker ps
```

> **Note:** The `WARNING: The requested image's platform (linux/amd64) does not match...` is normal on Apple Silicon Macs and can be safely ignored.

### Step 4: Install SQL Server Command-Line Tools

Homebrew makes this easy.

```bash
# Tap the Microsoft repository
brew tap microsoft/mssql-release https://github.com/Microsoft/homebrew-mssql-release

# Install the tools
brew update
brew install mssql-tools18

# Add the tools to your PATH (for the current session)
export PATH="/opt/homebrew/opt/mssql-tools18/bin:$PATH"
```

### Step 5: Create the Database and User

With SQL Server running in Docker, run these commands to set up the database.

```bash
# Create the database
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "CREATE DATABASE SecureSoftwareDatabase;"

# Create the login for the application
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "CREATE LOGIN appuser WITH PASSWORD = 'Replace_StrongPass1!';"

# Create the user in the database and grant permissions
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -d SecureSoftwareDatabase -Q "CREATE USER appuser FOR LOGIN appuser;"
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -d SecureSoftwareDatabase -Q "ALTER ROLE db_owner ADD MEMBER appuser;"

echo "✅ Database setup complete!"
```

### Step 6: Configure and Run the Application

Now, let's run the .NET application.

```bash
# Navigate to the main project folder
cd SecureSoftwareGroupProject

# Restore the project's NuGet packages
dotnet restore

# Install Entity Framework tools
dotnet tool install --global dotnet-ef 2>/dev/null || true
export PATH="$PATH:$HOME/.dotnet/tools"

# Create the database schema from the code (tables, columns, etc.)
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the application!
dotnet run
```

### Step 7: View the Application

Once the application is running, you will see the following output:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5179
```

Open your web browser and navigate to [**http://localhost:5179**](http://localhost:5179).

**Congratulations! The application is now running on your Mac.**

---

## 3. Troubleshooting

- **Problem:** `docker` command not found.
  - **Solution:** Ensure Docker Desktop is running.

- **Problem:** `dotnet` command not found.
  - **Solution:** Re-run `brew install --cask dotnet-sdk` or check your PATH.

- **Problem:** `sqlcmd` command not found.
  - **Solution:** Run `export PATH="/opt/homebrew/opt/mssql-tools18/bin:$PATH"` again.

- **Problem:** Error connecting to SQL Server (`Login timeout expired`).
  - **Solution:** Make sure the Docker container is running (`docker ps`). Wait a bit longer after starting it. Ensure your password is correct.

- **Problem:** `Invalid object name 'ProviderProfile'` when running the app.
  - **Solution:** You missed the `dotnet ef database update` step. Stop the app (`Ctrl+C`) and run it.

- **Problem:** Port 1433 or 5179 is already in use.
  - **Solution:** Find and stop the process using the port (`lsof -i :<port_number>`) or restart your machine.

---

## 4. Technology Stack

| Category | Technology |
| --- | --- |
| **Backend** | ASP.NET Core 8.0, Entity Framework Core 9.0 |
| **Database** | SQL Server 2022 (via Docker) |
| **Frontend** | Razor Pages, Bootstrap 5.3, Custom CSS |
| **Security** | BCrypt.Net-Next (Password Hashing), Anti-CSRF |
| **Testing** | xUnit |

---

## 5. Security Features Implemented

- **Password Security:** Passwords are never stored in plaintext. They are hashed using **BCrypt** with a work factor of 12.

- **Cross-Site Request Forgery (CSRF) Protection:** All forms are protected using anti-forgery tokens.

- **SQL Injection (SQLi) Prevention:** Entity Framework Core uses parameterized queries by default, eliminating the risk of SQLi.

- **Secure File Uploads:** Uploaded files are validated by extension and size. Filenames are randomized to prevent path traversal attacks.

- **Input Validation:** Server-side validation is enforced on all user inputs to prevent malicious data.

- **HTTPS Enforcement:** The application is configured to use HTTPS in production environments.

## If after everything the app still not running Do this and see this screen:

# Drop and recreate everything cleanly
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "DROP DATABASE IF EXISTS SecureSoftwareDatabase;"
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "DROP LOGIN IF EXISTS appuser;"

# Create the login FIRST
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "CREATE LOGIN appuser WITH PASSWORD = 'Replace_StrongPass1!';"

# Create the database
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "CREATE DATABASE SecureSoftwareDatabase;"

# Create the user and grant permissions
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -d SecureSoftwareDatabase -Q "CREATE USER appuser FOR LOGIN appuser;"
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -d SecureSoftwareDatabase -Q "ALTER ROLE db_owner ADD MEMBER appuser;"

# Now run the migrations (this will create the TABLES, not the database)
cd SecureSoftwareGroupProject
dotnet ef database update

# Run the app
dotnet run

## Then open
http://localhost:5179
