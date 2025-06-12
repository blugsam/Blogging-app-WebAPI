# Blogging App Web API

## Description

A RESTful API for a blogging platform, built with .NET 9, ASP.NET Core 9, Entity Framework Core, and PostgreSQL.

## Features

*   **Post Management:** Full CRUD (Create, Read, Update, Delete) operations for blog posts.
*   **Tag Management:** Creation of tags and association with posts.
*   **Slug Generation:** Automatic and unique slug generation for SEO-friendly URLs.
*   **Pagination:** Support for paginated listing of posts.
*   **Filtering:** Ability to filter posts by tag.
*   **Input Validation:** Robust request DTO validation using FluentValidation.
*   **Error Handling:** Centralized global error handling with `ProblemDetails` responses.
*   **Logging:** Structured logging implemented with Serilog, outputting to console (and configurable for files).
*   **API Documentation:** Interactive API documentation via Swagger (Swashbuckle).

## Tech Stack

*   **Backend:**
    *   .NET 9
    *   ASP.NET Core 9 Web API
    *   Entity Framework Core 9
*   **Database:**
    *   PostgreSQL
*   **Libraries & Tools:**
    *   AutoMapper
    *   FluentValidation
    *   Serilog
    *   Swashbuckle

## Getting Started

### Setup & Configuration

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/blugsam/Blogging-app-WebAPI.git
    cd Blogging-app-WebAPI
    ```

2.  **Configure Database Connection:**
    The application requires a PostgreSQL database.
    *   Create a new database.
    *   Create a database user with necessary permissions.
    *   Set up the connection string. The connection string is expected under `ConnectionStrings:DefaultConnection`.
        **Important:** For local development, use **User Secrets** to store your database password. Do **not** commit sensitive data to `appsettings.json`.
        Navigate to the `src/Blog.Api` directory and run:
        ```bash
        dotnet user-secrets init
        ```
        ```bash
        dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Host=localhost;Port=5432;Database=YOUR_DB_NAME;Username=YOUR_USERNAME;Password=YOUR_PASSWORD'
        ```
        Replace `YOUR_USERNAME` and `YOUR_PASSWORD` with your actual credentials.

3.  **Run Database Migrations:**
    Navigate to the `src/Blog.Infrastructure` directory and run:
    ```bash
    dotnet ef database update --startup-project ../Blog.Api
    ```
    This will create the necessary tables in your database.

### Running the API

1.  Navigate to the API project directory:
    ```bash
    cd src/Blog.Api
    ```
2.  Run the application:
    ```bash
    dotnet run
    ```
3.  The API will typically be available at `https://localhost:7226` (HTTPS) and `http://localhost:5052` (HTTP).
4.  Access the Swagger UI for interactive API documentation at `https://localhost:7226/swagger`.

## API Endpoints

*   **Posts:**
    *   `GET /api/posts`: Get all (published) posts (supports pagination and filtering by tag).
    *   `GET /api/posts/{slug}`: Get a specific post by its slug.
    *   `POST /api/posts`: Create a new post.
    *   `PUT /api/posts/{id}`: Update an existing post.
    *   `DELETE /api/posts/{id}`: Delete a post.
*   **Tags:**
    *   `GET /api/tags`: Get all tags.
