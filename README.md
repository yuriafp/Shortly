# URL Shortener API Project

This is an academic project consisting of a RESTful API for a URL shortening service, developed with ASP.NET Core.

The main goal is to convert a long URL into a short, unique code. When accessing the link with the short code, the user is redirected to the original URL, and the click count is tracked. The project was built with a focus on clean architecture and development best practices.

## Key Concepts Applied

The project was built with a focus on clean architecture and the application of software development best practices, including:

-   **Layered Architecture:** Clear separation of responsibilities between Controller (Presentation), Service (Business Logic), and Repository (Data Access).
-   **SOLID Principles:** Use of interfaces and Dependency Injection to create decoupled, testable, and maintainable code.
-   **Repository & Unit of Work Patterns:** Abstraction of the data layer and centralized control over database transactions using Entity Framework Core.
-   **Unique Short Code Generation:** Implementation of the `ID-to-Base62` strategy to ensure every short link is 100% unique, with no risk of collision.
-   **Unit Tests:** A test suite was created using xUnit and Moq to validate the business logic in isolation.

## Technologies Used

-   .NET 8 (or newer)
-   ASP.NET Core
-   Entity Framework Core
-   PostgreSQL
-   AutoMapper
-   xUnit, Moq, FluentAssertions

## How to Run the Project

Follow the steps below to run the application locally.

### Prerequisites

-   [.NET SDK](https://dotnet.microsoft.com/download)
-   [PostgreSQL](https://www.postgresql.org/download/) (a server running locally)

### Steps

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/yuriafp/Shortly.git
    cd Shortly
    ```

2.  **Configure the Database Connection:**
    Open the `appsettings.Development.json` file and adjust the `DefaultConnection` string with your PostgreSQL credentials.

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=shortly_db;Username=postgres;Password=your_password"
      }
    }
    ```

3.  **Create the Database:**
    Run the command below in the terminal from the project's root directory to apply the Entity Framework migrations and create the tables.
    ```bash
    dotnet ef database update
    ```

4.  **Run the Application:**
    ```bash
    dotnet run
    ```
    The API will be running at `https://localhost:7123` (or a similar port).

5.  **Access the API Documentation:**
    With the application running, access `https://localhost:7123/swagger` in your browser to see the interactive API documentation.

## How to Use the API (Endpoints)

#### Public Redirect Endpoint

-   **`GET /{shortCode}`**
    -   Redirects the user to the corresponding original URL.
    -   **Example:** Access `https://localhost:7123/my-code` in the browser.

#### Management Endpoints (`/api/urls`)

-   **`POST /api/urls`**
    -   **Description:** Creates a new shortened link.
    -   **Request Body (JSON):**
        ```json
        {
          "originalUrl": "https://a-very-long-url-to-be-shortened.com"
        }
        ```
    -   **Success Response:** `201 Created` with the data of the created link.

-   **`GET /api/urls/{shortCode}`**
    -   **Description:** Returns the details of a shortened link (original URL, creation date, clicks, etc.).

-   **`DELETE /api/urls/{shortCode}`**
    -   **Description:** Performs a "soft delete" on a link, marking it as expired.
    -   **Success Response:** `204 No Content`.

## How to Run the Tests

To run the unit test suite, navigate to the solution's root directory and execute the command:

```bash
dotnet test
