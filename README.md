# 10xCards

## Project Description
10xCards is a web application designed to automate the creation of high-quality educational flashcards using artificial intelligence. The system not only generates flashcards based on a provided text (ranging from 1000 to 10000 characters) using summarization and editing techniques but also allows users to manually create, edit, view, and delete flashcards. This dual approach enables users to save time and personalize their study materials while supporting the spaced repetition learning method.

## Tech Stack
- **Frontend:** Blazor with MudBlazor components and simple CSS for fast and effective styling.
- **Backend:** .NET 8 for building robust applications, with PostgreSQL 17 as the database.
- **AI Integration:** Communication with various AI models via Openrouter.ai for efficient and cost-effective flashcard generation.
- **CI/CD & Hosting:** Github Actions for automated pipelines and DigitalOcean for containerized hosting.
- **Testing:** 
  - Unit Tests: xUnit, FluentAssertions, AutoFixture, and TestContainers for database testing
  - E2E Tests: Playwright for end-to-end automated browser testing
  - Additional: bUnit for Blazor component testing, NBomber for performance testing

## Getting Started Locally
1. **Clone the repository:**
   ```sh
   git clone https://github.com/your-username/10xCards.git
   cd 10xCards
   ```

2. **Install dependencies:**
   - Ensure [.NET 9](https://dotnet.microsoft.com/download) and [PostgreSQL 17](https://www.postgresql.org/download/) are installed.
   - Restore project dependencies:
     ```sh
     dotnet restore
     ```

3. **Set up the environment:**
   - Copy the `.env.example` to `.env` and configure your environment variables.
   - Set up your PostgreSQL database and update the connection string accordingly.

4. **Run the application:**
   ```sh
   dotnet run
   ```
   The application will start locally, and you can access it via your browser.

## Available Scripts
- **Build the project:**
  ```sh
  dotnet build
  ```
- **Run the application locally:**
  ```sh
  dotnet run
  ```
- **Run tests:**
  ```sh
  dotnet test                           # Run all tests
  dotnet test TenXCards.API.Tests       # Run only unit tests
  dotnet test TenXCards.E2E.Tests       # Run only E2E tests
  ```

## Project Scope
- **AI Generated Flashcards:** Automatically generate flashcards based on user-submitted text using summarization techniques.
- **Manual Flashcard Creation:** Allow users to manually create, edit, view, and delete flashcards.
- **User Accounts:** Simple registration and authentication for secure access to personal flashcards.
- **Integration with Spaced Repetition:** Future integration with a spaced repetition algorithm to enhance learning (not implemented).

## Project Status
The project is currently in the MVP stage. Core functionalities such as flashcard generation and user management have been implemented, and further improvements including enhanced UI/UX and performance optimizations are underway.

## License
This project is licensed under the terms specified in the LICENSE file.
