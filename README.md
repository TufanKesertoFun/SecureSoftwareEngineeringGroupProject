# Secure Software Engineering Group Project:

**`Secure Home Application`**

This document provides a comprehensive, high-definition guide to the **SecureHome** home services application, a project for the Secure Software Engineering course. It covers the project's architecture, security features, and meticulously detailed instructions for setup, execution, and verification on both Windows and macOS/Linux environments, ensuring any user can run the application successfully.

---

## Part 1: Detailed Project Explanation

### 1.1. Project Overview

**SecureHome** is a secure-by-design web application built using ASP.NET Core 8.0 Razor Pages. It serves as a platform connecting users with home service professionals. The paramount focus of the project is the implementation of robust security practices to protect user data, ensure privacy, and build a trustworthy platform. The application features a clean, responsive user interface and a backend built on modern, secure technologies.

### 1.2. Project Structure

The solution is organized into three main projects:

| Project | Purpose |
| --- | --- |
| `SecureSoftwareGroupProject` | The main ASP.NET Core Razor Pages web application. This contains all the pages, models, data access logic, and security configurations. |
| `SecureSoftwareGroupProject.Tests` | A dedicated xUnit test project for unit and integration testing of the main application's components, ensuring code quality and correctness. |
| `FuzzingApplicationforSecurity` | A console application that uses the SharpFuzz library to perform fuzz testing on a sample authentication service, identifying potential vulnerabilities from malformed inputs. |

### 1.3. Key Features

- **User Authentication:** Secure user registration and login system using **BCrypt** for password hashing, preventing plaintext password storage.

- **Role-Based Access Control (RBAC):** The system is designed to differentiate between standard users and administrators, with pages and actions restricted based on user roles.

- **Provider Profiles:** Service providers can create and manage their profiles, showcasing their services, rates, and contact information.

- **Customer Reviews & Ratings:** Customers can submit reviews and ratings for services they have received, including the ability to upload images securely.

- **Secure File Uploads:** A dedicated mechanism for uploading images that includes validation for file type, size, and sanitization of filenames to prevent path traversal and other file-based attacks.

- **Data Protection:** The application uses parameterized queries through Entity Framework Core to prevent SQL injection attacks.

- **Fuzz Testing:** The project includes a fuzzing harness to test the resilience of the authentication logic against unexpected and malformed inputs.

### 1.4. Security Features Implemented

The following table summarizes the key security features and their implementation:

| Feature | Implementation Details |
| --- | --- |
| **Password Security** | Passwords are never stored in plaintext. They are hashed using **BCrypt.Net-Next** with a work factor of 12. This is a strong, adaptive hashing algorithm that is resistant to brute-force and rainbow table attacks. |
| **SQL Injection (SQLi) Prevention** | The application uses **Entity Framework Core** for all database interactions. EF Core uses parameterized queries by default, which means that user input is treated as data, not as executable SQL code, thus neutralizing SQLi threats. |
| **Cross-Site Request Forgery (CSRF) Protection** | ASP.NET Core's built-in anti-forgery token system is enabled. All forms that modify data (POST requests) are automatically protected, ensuring that requests originate from the application itself and not from a malicious third-party site. |
| **Secure File Uploads** | A multi-layered validation process is applied to all file uploads:
- **File Extension Whitelisting:** Only specific image extensions (`.jpg`, `.png`, `.gif`, `.webp`) are permitted.
- **File Size Limits:** A maximum file size of 2MB is enforced to prevent denial-of-service attacks.
- **Randomized Filenames:** Uploaded files are saved with a randomly generated GUID as the filename to prevent path traversal attacks and filename conflicts. |
| **Input Validation** | Both client-side and server-side validation are implemented. Server-side validation, using Data Annotations on the models, is the primary defense against malicious input, ensuring that all data conforms to expected formats and constraints before being processed. |
| **HTTPS Enforcement** | The application is configured to use `UseHttpsRedirection()` and `UseHsts()` in production, forcing all communication between the client and server to be encrypted over HTTPS. |
| **Error Handling & Information Leakage** | Generic error pages are shown to the user in production, while detailed exception information is logged and only made available in the development environment. This prevents the leakage of sensitive system information. |
| **Authentication & Authorization** | The application uses cookie-based authentication. The `[Authorize]` attribute is used to protect pages and resources, ensuring that only authenticated and authorized users can access them. |

### 1.5. Page Descriptions

- **/Index**: The public-facing landing page of the application.

- **/Login**: The user login page. It authenticates users against the database using their username and password.

- **/Signup**: The user registration page. It allows new users to create an account, with password complexity rules enforced.

- **/Logout**: Clears the user's session and redirects them to the homepage.

- **/Clients**: An authorized page that displays a list of all customers from the `CustomerBalance` table. It includes search and filtering capabilities.

- **/Customer**: A placeholder page for customer-specific information.

- **/CustomerBalance**: A page for managing customer balances and credit limits.

- **/CustomerReviews**: A page where users can view, create, edit, and delete reviews for service providers. This page includes the secure image upload functionality.

- **/ProviderProfiles**: A page to view and manage the profiles of service providers.

- **/Privacy**: A standard privacy policy page.

- **/Error**: A user-friendly error page displayed in case of unhandled exceptions.

### 1.6. How to Run Tests

The project includes a suite of xUnit tests to verify the functionality of the models and pages. To run the tests:

1. Open a terminal or command prompt.

1. Navigate to the `SecureSoftwareGroupProject.Tests` directory:

1. Run the following command from the project root: `dotnet test`

This will build the test project and execute all the tests, providing a summary of the results.

### 1.7. Fuzzing

A fuzzing application (`FuzzingApplicationforSecurity`) is included to test the robustness of a sample authentication service. It uses the **SharpFuzz** library to generate a wide range of inputs to try and find crashes or unexpected behavior.

- **How it Works:** The fuzzer generates random and mutated strings for the `username` and `password` fields and passes them to a mock `AuthService.Authenticate` method. It looks for exceptions or other anomalies that could indicate a vulnerability.

- **Findings:** The log file `FuzzingApplicationforSecurity/findings/fuzz_log_cli.txt` shows the output of a sample fuzzing run. The key metrics are:
  - `cov` (coverage): The number of code branches discovered by the fuzzer.
  - `corp` (corpus): The number of interesting test cases saved for future runs.
  - `exec/s`: The number of executions per second. The goal of fuzzing is to maximize code coverage and uncover edge cases that might be missed by traditional testing.

---

## Part 2: How to Run on Windows

This project is built on ASP.NET Core and is well-suited for a Windows development environment using Visual Studio and SQL Server.

### Prerequisites

- **Windows 10 or 11**

- **Visual Studio 2022** with the **ASP.NET and web development** workload installed.

- **.NET 8.0 SDK** (usually included with the Visual Studio workload).

- **SQL Server 2019 or later** (Developer or Express edition).

- **SQL Server Management Studio (SSMS)**.

### Step 1: Clone the Repository

Clone the project from GitHub to a local directory.

```bash
git clone https://github.com/TufanKesertoFun/SecureSoftwareEngineeringGroupProject.git
cd SecureSoftwareEngineeringGroupProject
```

### Step 2: Set Up the Database in SQL Server

1. **Open SQL Server Management Studio (SSMS )** and connect to your local SQL Server instance.

1. In the **Object Explorer**, right-click on **Databases** and select **New Database**.

1. Name the database `SecureSoftwareDatabase` and click **OK**.

1. Now, create a login for the application. Right-click on **Security** > **Logins** and select **New Login...**.

1. Enter `appuser` as the **Login name**.

1. Select **SQL Server authentication**.

1. Enter a strong password, for example, `Replace_StrongPass1!`. You must use this same password in the `appsettings.json` file.

1. Uncheck **Enforce password policy** to use this specific password.

1. In the **User Mapping** page, check the box next to the `SecureSoftwareDatabase` database.

1. In the **Database role membership** panel below, check `db_owner`.

1. Click **OK** to create the login.

### Step 3: Configure the Connection String

1. Open the solution file (`SecureSoftwareGroupProject.sln`) in Visual Studio.

1. In the **Solution Explorer**, find and open the `appsettings.json` file within the `SecureSoftwareGroupProject` project.

1. Modify the `DefaultConnection` string to match your SQL Server setup. If you used the settings from the previous step, it should look like this:
  - **Server**: Your SQL Server instance name (e.g., `localhost`, `.\SQLEXPRESS`).
  - **Password**: The password you set for the `appuser` login.

### Step 4: Create the Database Schema

1. In Visual Studio, open the **Package Manager Console** (`View` > `Other Windows` > `Package Manager Console`).

1. Ensure the `SecureSoftwareGroupProject` is selected as the **Default project** in the console's dropdown menu.

1. Run the following command to create the database schema (tables, columns, etc.) based on the application's models:

### Step 5: Run the Application and Verify

1. **Press F5** or click the green play button (labeled with `https` ) in the Visual Studio toolbar. This action will:
  - Build the project.
  - Start the Kestrel web server.
  - Launch your default web browser.

1. **Verification:**
  - The browser will open and navigate to a URL like `https://localhost:XXXX` (the port number is chosen by Visual Studio ).
  - You should see the **SecureService** home page, with options to "Login" and "Sign Up".
  - The Visual Studio console window will show logs from the application, including a line like `Now listening on: https://localhost:XXXX`.

1. **To stop the application**, simply close the console window or press the red stop button in Visual Studio.

### Troubleshooting (Windows )

- **Error: "A network-related or instance-specific error occurred while establishing a connection to SQL Server."**
  - **Solution:** Ensure your SQL Server instance is running. Check the server name in your connection string. If you're using SQL Express, the server name is often `.\SQLEXPRESS`. Also, check that your firewall is not blocking the SQL Server port (default is 1433).

- **Error: "Cannot open database 'SecureSoftwareDatabase' requested by the login. The login failed."**
  - **Solution:** Double-check the `User ID` and `Password` in your `appsettings.json`. Make sure the `appuser` login has been correctly mapped to the `SecureSoftwareDatabase` with the `db_owner` role in SSMS.

- **Migrations fail (****`Update-Database`****):**
  - **Solution:** Ensure the `SecureSoftwareGroupProject` is selected as the default project in the Package Manager Console. Try rebuilding the solution (`Build` > `Rebuild Solution`) and then run the command again.

---

## Part 3: How to Run on macOS & Linux

For macOS and Linux, the recommended approach is to run SQL Server in a Docker container.

### Prerequisites

- **macOS 11+ or a modern Linux distribution**

- **Homebrew** (for macOS) or a package manager like `apt` or `yum` (for Linux).

- **.NET 8.0 SDK**

- **Docker Desktop** (for macOS) or **Docker Engine** (for Linux).

### Step 1: Install Prerequisites

Open your terminal and install the necessary tools.

**On macOS:**

```bash
# Install .NET 8.0 SDK
brew install --cask dotnet-sdk

# Install Docker Desktop
brew install --cask docker
```

**On Linux (Debian/Ubuntu):**

```bash
# Install .NET 8.0 SDK
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Install Docker
sudo apt-get install -y docker.io
```

After installing, launch Docker and ensure it is running.

### Step 2: Clone the Repository

```bash
git clone https://github.com/TufanKesertoFun/SecureSoftwareEngineeringGroupProject.git
cd SecureSoftwareEngineeringGroupProject
```

### Step 3: Set Up and Run SQL Server via Docker

```bash
# Stop and remove any old container to avoid conflicts
docker rm -f sqlserver 2>/dev/null || true

# Run the SQL Server 2022 container
docker run -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=Replace_StrongPass1! -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# Wait for SQL Server to initialize
echo "Waiting for SQL Server to start..."
sleep 30
```

### Step 4: Create the Database and User

You will need the SQL Server command-line tools.

**On macOS:**

```bash
brew tap microsoft/mssql-release
brew update
brew install mssql-tools18
```

**On Linux (Debian/Ubuntu ):**

```bash
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
curl https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs )/prod.list | sudo tee /etc/apt/sources.list.d/mssql-release.list
sudo apt-get update
sudo ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev
```

**Now, run the following commands to set up the database:**

```bash
# Add tools to your PATH
export PATH="$PATH:/opt/mssql-tools18/bin"

# Create the database
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "CREATE DATABASE SecureSoftwareDatabase;"

# Create the login
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -Q "CREATE LOGIN appuser WITH PASSWORD = 'Replace_StrongPass1!';"

# Create the user and grant permissions
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -d SecureSoftwareDatabase -Q "CREATE USER appuser FOR LOGIN appuser;"
sqlcmd -S localhost -U sa -P Replace_StrongPass1! -C -d SecureSoftwareDatabase -Q "ALTER ROLE db_owner ADD MEMBER appuser;"

echo "✅ Database setup complete!"
```

### Step 5: Configure, Build, and Run the Application

1. Navigate to the main project folder:

1. The `appsettings.json` is already configured for this setup. Ensure the password matches the one you used.

1. Install the Entity Framework Core tools:

1. Apply the database migrations:

1. Run the application using the `dotnet run` command. This command builds the project and starts the web server.

1. **Verification:**
  - The terminal will show logs from the application, including a line like `Now listening on: http://localhost:5179` and `https://localhost:7179`.
  - Open your web browser and navigate to [**https://localhost:7179**](https://localhost:7179) (or the HTTPS port specified in the terminal output ).
  - You should see the **SecureService** home page.

1. **To stop the application**, press `Ctrl+C` in the terminal.

### Troubleshooting (macOS/Linux)

- **Error: ****`dotnet: command not found`**
  - **Solution:** The .NET SDK is not in your PATH. Re-run the installation command or add the dotnet installation directory to your shell's profile file (e.g., `.zshrc`, `.bash_profile`).

- **Error: ****`docker: command not found`**
  - **Solution:** Ensure Docker Desktop (macOS) or Docker Engine (Linux) is installed and running.

- **Error connecting to SQL Server (****`Login timeout expired`****)**
  - **Solution:** Verify the Docker container is running with `docker ps`. If it's not listed, start it again. Ensure the password in `appsettings.json` matches the one used in the `docker run` command.

- **Error: ****`Invalid object name 'Users'`**** when running the app.**
  - **Solution:** You missed the `dotnet ef database update` step. Stop the app (`Ctrl+C`), run the command, and then start the app again.

****

