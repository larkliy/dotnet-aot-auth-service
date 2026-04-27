# .NET 10 Native AOT Auth Microservice

![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![AOT](https://img.shields.io/badge/Native_AOT-Ready-success)
![Docker](https://img.shields.io/badge/Docker-Supported-blue)

A high-performance, lightweight Authentication Microservice built with **.NET 10**, tailored for **Native AOT (Ahead-of-Time) compilation**. This service is designed to be extremely fast, memory-efficient, and container-friendly.

## 🚀 Features

*   **Native AOT Ready:** Compiled to native machine code. Minimal footprint, instant startup time, no JIT compilation overhead.
*   **JWT & Refresh Tokens:** Secure authentication flow using standard JWT access tokens and long-lived refresh tokens.
*   **Source-Generated & Reflection-Free:** Heavily utilizes .NET 10 source generators for JSON serialization (`JsonSerializerContext`), configuration validation, and Dapper (`Dapper.AOT`).
*   **Fail-Fast Configuration:** `IOptions` validation runs on application startup to ensure all critical environment variables (like JWT Keys) are present before serving requests.
*   **Secure by Design:** Passwords are mathematically hashed using `BCrypt.Net-Next`.
*   **Docker Optimized:** Uses a multi-stage Dockerfile resulting in a tiny, lightweight runtime container (`~30MB` depending on OS base).

## 🛠 Tech Stack

*   **Framework:** .NET 10 / ASP.NET Core Minimal APIs
*   **Database:** SQLite
*   **ORM:** Dapper (via `Dapper.AOT`)
*   **Security:** BCrypt, System.IdentityModel.Tokens.Jwt

## 📦 Getting Started

### Local Development

1. Ensure you have the [.NET 10 SDK](https://dotnet.microsoft.com/) installed.
2. Clone the repository:
   ```bash
   git clone https://github.com/larkliy/dotnet-aot-auth-service.git
   cd dotnet-aot-auth-service
   ```
3. Set your secret JWT key in `appsettings.json` (or via User Secrets / Environment Variables). *The key must be at least 32 characters long.*
4. Run the application:
   ```bash
   dotnet run
   ```
   *Note: The SQLite database (`users.db`) will be created automatically on the first run.*

### Running with Docker

Because this application uses Native AOT, the Docker build compiles the app into a Linux binary. To persist the SQLite database across container restarts, we mount a volume.

1. **Build the image:**
   ```bash
   docker build -t auth-service .
   ```

2. **Run the container (with volume mapping):**
   ```bash
   # Create a local folder to store the database
   mkdir -p $(pwd)/data

   # Run the container mapping the local folder to /app/data
   docker run -d \
     -p 8080:8080 \
     -v $(pwd)/data:/app/data \
     -e ConnectionStrings__DefaultConnection="Data Source=/app/data/users.db" \
     --name auth-service \
     auth-service
   ```

## 🔌 API Endpoints

Base URL: `/auth`

### 1. Register a new user
**`POST /auth/register`**

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SuperSecretPassword123"
}
```
**Responses:**
*   `200 OK` - Successfully registered.
*   `400 Bad Request` - Validation error (e.g., password too short).
*   `409 Conflict` - Email already exists.

### 2. Login
**`POST /auth/login`**

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SuperSecretPassword123"
}
```
**Responses:**
*   `200 OK` - Successfully authenticated.
    ```json
    {
      "accessToken": "eyJhbGciOiJIUzI1NiIs...",
      "refreshToken": "vXyZ/AbCdE123..."
    }
    ```
*   `400 Bad Request` - User does not exist.
*   `401 Unauthorized` - Invalid password.

### 3. Refresh Token
**`POST /auth/refresh`**

**Request Body:**
```json
{
  "accessToken": "expired_jwt_token_here",
  "refreshToken": "your_refresh_token_here"
}
```
**Responses:**
*   `200 OK` - Returns a new set of tokens.
    ```json
    {
      "accessToken": "NEW_eyJhbGciOiJIUzI1NiIs...",
      "refreshToken": "NEW_vXyZ/AbCdE123..."
    }
    ```
*   `401 Unauthorized` - Invalid or expired refresh token.

## ⚙️ Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=users.db"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_MIN_32_CHARS_LONG",
    "Audience": "AuthService",
    "Issuer": "AuthService"
  }
}
```

## 🤝 Contributing
Contributions, issues, and feature requests are welcome! Feel free to check [issues page](https://github.com/larkliy/dotnet-aot-auth-service/issues).

## 📝 License
This project is [MIT](LICENSE) licensed.
