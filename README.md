# 10xCards

## Project Description

10xCards is a web application designed to automate the generation of high-quality educational flashcards using AI. By processing text inputs (between 1000 and 10000 characters), the system leverages summarization and content editing techniques to create flashcards that can also be manually edited. This solution streamlines the spaced repetition learning process by saving users the time required to create flashcards manually.

## Tech Stack

- **Astro 5** – for fast, efficient static and hybrid rendering  
- **React 19** – for interactive UI components  
- **TypeScript 5** – for type-safe development  
- **Tailwind 4** – for rapid and responsive styling  
- **Shadcn/ui** – for pre-built UI components  
- **Supabase** – for backend services including authentication and database interactions

## Getting Started Locally

1. **Ensure the correct Node.js version is used**  
   Use [Node Version Manager (nvm)] to switch to the required version:
   ```bash
   nvm use 22.14.0
   ```
2. **Install dependencies**  
   From the project root, run:
   ```bash
   npm install
   ```
3. **Start the development server**  
   Launch the project locally with:
   ```bash
   npm run dev
   ```

## Available Scripts

- **`npm run dev`** – Start the Astro development server.
- **`npm run build`** – Build the project for production.
- **`npm run preview`** – Preview the production build locally.
- **`npm run lint`** – Run ESLint across the project.
- **`npm run lint:fix`** – Automatically fix linting issues.
- **`npm run format`** – Format code using Prettier.

## Project Scope

- **AI-based Flashcard Generation:**  
  Users can input text (1000–10000 characters), which the AI processes to generate flashcards. The system utilizes summarization and content editing techniques to create an initial version that users can further edit.

- **Manual Flashcard Creation:**  
  Provides an interface for users to manually create, edit, view, and delete flashcards, ensuring full control over the content.

- **User Management:**  
  Features simple registration and login mechanisms for secure flashcard management.

- **Spaced Repetition Integration:**  
  Flashcards integrate with a simple spaced repetition algorithm to enhance the learning process.

## Project Status

This project is currently in the MVP stage. The core functionalities are implemented with ongoing improvements and refinements expected as user feedback is collected.

## License

This project is licensed under the [MIT License](LICENSE).