# AI Rules for {{project-name}}

This document provides best practices and guidelines for working on backend applications using .NET C#, frontend applications with Blazor, designing clear and useful user interfaces, and proper database design practices with Postgres.

---

## Project structure

- Backend - TenXCards.API
- Frontend - TenXCards.Frontend

---

## Table of Contents

- Backend Best Practices
- Frontend Best Practices
- User Interface Design Guidelines
- Database Design Best Practices
- General Recommendations

---

## Backend Best Practices

### Clean Code for Backend Applications (.NET C#)

- **Separation of Concerns**:  
  Divide your code into layers (e.g., API, business logic, data access). Each layer should have a well-defined responsibility.

- **SOLID Principles**:  
  Follow SOLID design principles to make your code maintainable and scalable.

- **Error Handling & Logging**:  
  - Use try-catch blocks around critical code.  
  - Implement centralized logging using frameworks like Serilog or NLog.  
  - Provide meaningful error messages for debugging and production troubleshooting without exposing sensitive information.

- **Dependency Injection**:  
  Use built-in ASP.NET Core dependency injection to manage service lifetimes.

- **Unit Testing**:  
  Design your code in a testable manner and include unit tests using frameworks like xUnit or NUnit.

- **Code Readability**:  
  - Write self-explanatory and descriptive method and variable names.  
  - Separate complex logic into smaller, reusable methods.  
  - Use comments and XML documentation where necessary.

- **Performance Optimization**:  
  - Emphasize asynchronous programming (async/await) where possible.  
  - Profile and optimize performance hotspots.

- **Security**:  
  - Validate all input data on the server side.  
  - Implement proper authentication and authorization mechanisms, such as ASP.NET Core Identity or OAuth providers.

---

## Frontend Best Practices

### Frontend project

- **Main project files**:
  - _Imports.razor – A file importing namespaces and other resources used in the Blazor application.
  - App.razor – The main component of the Blazor application, defining the layout and router of the application.
  - Program.cs – The entry point of the application, configuring services and starting the Blazor application.
  - TenXCards.Frontend.csproj – The .NET project file defining dependencies and project settings.
- **Folders**:
  - bin/ and obj/ – Automatically generated folders by the .NET build system, containing binaries and temporary files.
  - Layout/ – Contains application layouts, including:
  - Models/ – Contains data models and DTOs including:
  - Pages/ – A folder containing Blazor pages and components:
  - Services/ – Service implementations including:
  - Validators/ – Validation logic:
  - Properties/ – Contains project configuration files (launchSettings.json)
  - wwwroot/ – A public folder containing static resources:

### Clean Code for Frontend Applications (Blazor)

- **Component-based Architecture**:  
  - Develop UI using reusable components.  
  - Break down complex pages into smaller, manageable Blazor components.

- **Data Binding & State Management**:  
  - Use two-way data binding carefully to ensure state consistency.  
  - Leverage state management patterns or libraries when the application state gets complex.

- **Maintainability**:  
  - Keep component logic separate from UI rendering.  
  - Use code-behind files for logic to keep .razor files clean.

- **Performance**:  
  - Avoid unnecessary re-rendering by using Blazor component lifecycle methods effectively.  
  - Consider leveraging `@key` in loops to optimize rendering.

- **Testing**:  
  - Create component tests using frameworks like bUnit to ensure UI components behave as expected.

### Tailwind

- Use the @layer directive to organize styles into components, utilities, and base layers
- Use arbitrary values with square brackets (e.g., w-[123px]) for precise one-off designs
- Implement the Tailwind configuration file for customizing theme, plugins, and variants
- Leverage the theme() function in CSS for accessing Tailwind theme values
- Implement dark mode with the dark: variant
- Use responsive variants (sm:, md:, lg:, etc.) for adaptive designs
- Leverage state variants (hover:, focus-visible:, active:, etc.) for interactive elements

---

## User Interface Design Guidelines

### Designing a Clear and Usable User Interface

- **Consistency**:  
  - Keep visual consistency with colors, typography, and spacing.  
  - Follow a design system or pattern library.

- **Accessibility**:  
  - Use semantic HTML elements and ARIA roles where appropriate.  
  - Ensure color contrast meets accessibility standards.  
  - Provide keyboard navigation support and focus management.

- **Feedback and Responsiveness**:  
  - Inform users of actions with visual feedback (loading indicators, button states).  
  - Ensure the UI adapts to various screen sizes and orientations (responsive design).

- **Simplicity**:  
  - Keep the interface simple and intuitive.  
  - Minimize unnecessary elements and provide a clear hierarchy of information.

- **User Testing**:  
  - Regularly perform usability testing and iterate based on user feedback.

---

## Database Design Best Practices

### Best Practices for Postgres

- **Normalization and Schema Design**:  
  - Normalize your database to avoid redundancy but consider denormalization for performance when needed.  
  - Use clear, consistent naming conventions for tables, columns, and indexes.

- **Indexes and Performance**:  
  - Create indexes on columns used frequently in WHERE clauses, JOIN operations, and as foreign keys.  
  - Regularly analyze and optimize query performance.

- **Transactions and Concurrency**:  
  - Use transactions to ensure data consistency.  
  - Implement appropriate isolation levels to handle concurrent access effectively.

- **Security**:  
  - Restrict database access with proper roles and permissions.  
  - Use secure connections (SSL/TLS) and avoid storing sensitive information in plaintext.

- **Backup and Recovery**:  
  - Plan for regular backups and test your recovery process to mitigate data loss risks.

- **Monitoring and Maintenance**:  
  - Regularly monitor database performance and tune queries and indexes.  
  - Plan for maintenance tasks like vacuuming and analyzing tables.

---

## Testing

### Unit Testing Best Practices

- **Naming Conventions:**
  - Test classes should be named according to the system under test (SUT).
  - Test methods should follow the pattern: MethodName_StateUnderTest_ExpectedBehavior or Should_ExpectedBehavior_When_StateUnderTest.
  - Use descriptive names for test cases using the [Theory] attribute.

- **Test Structure (AAA):**
  - **Arrange:** Set up data and configuration.
  - **Act:** Execute the method under test.
  - **Assert:** Verify results with a single logical assertion or use BeEquivalentTo for multiple checks.

- **xUnit Best Practices:**
  - Use [Fact] for single scenarios and [Theory] for parameterized tests.
  - Utilize IClassFixture or CollectionFixture for expensive setups.
  - Ensure tests are isolated from dependencies.
  - Group tests using the [Trait] attribute.

- **FluentAssertions:**
  - Use semantic assertions for collections, exceptions, and object comparisons.
  - Prefer using BeEquivalentTo over chaining multiple Should() calls.

- **AutoFixture:**
  - Automate test data generation to minimize manual object creation.
  - Globally customize the data generator to avoid “magic” values.

- **TestContainers for Integration Tests:**
  - Use IClassFixture to run a database container only once.
  - Reset the database state before each test or use a new container.

- **WireMock.NET:**
  - Create dedicated stubs to simulate external APIs.
  - Verify that HTTP calls are executed as expected.

- **Mocking (NSubstitute/Moq):**
  - Design based on interfaces.
  - Verify interactions with dependencies.

- **Asynchronous Tests:**
  - Use async/await for asynchronous methods.
  - Test exceptions in asynchronous methods properly.

- **General QA Principles:**
  - Tests should be fast (ideally less than 50 ms) and have a single responsibility.
  - Avoid real connections to external systems (use WireMock or interfaces).
  - Limit time-based assertions (avoid sleep calls).
  - Ensure tests are CI-compatible (no GUI interactions and clear error messages).
  - Focus on detecting significant edge cases and following TDD rather than achieving high code coverage.

  ### Guidelines for E2E

#### PLAYWRIGHT

- Initialize configuration only with Chromium/Desktop Chrome browser
- Use browser contexts for isolating test environments
- Implement the Page Object Model for maintainable tests
- Use locators for resilient element selection
- Leverage API testing for backend validation
- Implement visual comparison with expect(page).toHaveScreenshot()
- Use the codegen tool for test recording
- Leverage trace viewer for debugging test failures
- Implement test hooks for setup and teardown
- Use expect assertions with specific matchers
- Leverage parallel execution for faster test runs

---

## General Recommendations

- **Continuous Integration/Continuous Deployment (CI/CD)**:  
  Automate your build and deployment processes to catch issues early and deploy reliably.

- **Branching Strategy**:  
  Follow a clear branching strategy such as Gitflow for version control.

- **Documentation**:  
  - Maintain up-to-date documentation for your code and architectural decisions.  
  - Ensure your API documentation is clear, perhaps using tools like Swagger.

- **Code Reviews**:  
  Regularly perform peer code reviews to maintain code quality and share knowledge among the team.

---

By following these guidelines, developers can build reliable, maintainable, and scalable applications with .NET C#, Blazor, and Postgres while ensuring a clear and accessible user interface.