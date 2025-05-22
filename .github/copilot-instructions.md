# AI Rules for {{project-name}}

This document provides best practices and guidelines for working on backend applications using .NET C#, frontend applications with Blazor, designing clear and useful user interfaces, and proper database design practices with Postgres.

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