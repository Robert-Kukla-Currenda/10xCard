# REST API Plan

## 1. Resources

- **Users**  
  Represents registered users. Maps to the `users` table.

- **Cards**  
  Represents flashcards created for learning. Maps to the `cards` table.  
  *Validation:*  
  - `original_content_id`: Required.
  - `front` text length: 1–1000 characters.  
  - `back` text length: 1–5000 characters.  
  - `generated_by` value must be either `"AI"` or `"human"`.

- **Error Logs**  
  Represents error details associated with a card. Maps to the `error_logs` table.

## 2. Endpoints

### 2.1. Users

#### Register User
- **Method:** POST  
- **URL:** `/users/register`  
- **Description:** Creates a new user account.  
- **Request Payload:**
    ```json
    {
      "email": "user@example.com",
      "first_name": "John",
      "last_name": "Doe",
      "password": "PlainTextPassword"
    }
    ```
- **Response:**
    ```json
    {
      "id": 1,
      "email": "user@example.com",
      "first_name": "John",
      "last_name": "Doe",
      "created_at": "2025-04-19T12:00:00Z"
    }
    ```
- **Success Codes:** 201 Created  
- **Error Codes:** 400 Bad Request, 409 Conflict (if email exists)

#### User Login
- **Method:** POST  
- **URL:** `/users/login`  
- **Description:** Authenticates a user and returns a JWT token.  
- **Request Payload:**
    ```json
    {
      "email": "user@example.com",
      "password": "PlainTextPassword"
    }
    ```
- **Response:**
    ```json
    {
      "token": "JWT_TOKEN_HERE",
      "user": {
        "id": 1,
        "email": "user@example.com",
        "first_name": "John",
        "last_name": "Doe"
      }
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized

#### Get Current User Profile
- **Method:** GET  
- **URL:** `/users/me`  
- **Description:** Retrieves the profile of the authenticated user.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Response:**
    ```json
    {
      "id": 1,
      "email": "user@example.com",
      "first_name": "John",
      "last_name": "Doe",
      "created_at": "2025-04-19T12:00:00Z"
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized

### 2.2. Original Contents

#### Create Original Content
- **Method:** POST  
- **URL:** `/original-contents`  
- **Description:** Save new original content that will be used to generate flashcards.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Request Payload:**
    ```json
    {
      "content": "Long text content that will be used to generate flashcards..."
    }
    ```
- **Field Validations:**
    - `content`: Required, must be between 1000 and 10000 characters
- **Response:**
    ```json
    {
      "id": 1,
      "user_id": 1,
      "content": "Long text content that will be used to generate flashcards...",
      "created_at": "2025-04-19T12:00:00Z"
    }
    ```
- **Success Codes:** 201 Created  
- **Error Codes:** 
    - 400 Bad Request (validation errors)
    - 401 Unauthorized

#### Get Original Contents List
- **Method:** GET  
- **URL:** `/original-contents`  
- **Description:** Retrieves a paginated list of user's original contents.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Query Parameters:**
    - `page` (optional, default: 1)
    - `limit` (optional, default: 20)
    - `sort` (optional, e.g., `created_at_desc`)
- **Response:**
    ```json
    {
      "items": [
        {
          "id": 1,
          "user_id": 1,
          "content": "Long text content...",
          "created_at": "2025-04-19T12:00:00Z",
          "cards": [
            {
              "id": 1,
              "user_id": 1,
              "front": "Generated summary question...",
              "back": "Generated detailed answer...",
              "generated_by": "AI",
              "created_at": "2025-04-19T12:10:00Z"
            }
          ]
        }
      ],
      "pagination": {
        "page": 1,
        "limit": 20,
        "total": 35
      }
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized

#### Get Single Original Content
- **Method:** GET  
- **URL:** `/original-contents/{id}`  
- **Description:** Retrieves a specific original content by its ID.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Response:**
    ```json
    {
      "id": 1,
      "user_id": 1,
      "content": "Long text content...",
      "created_at": "2025-04-19T12:00:00Z"
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized, 404 Not Found

#### Delete Original Content
- **Method:** DELETE  
- **URL:** `/original-contents/{id}`  
- **Description:** Deletes the specified original content and all associated cards.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Response:**
    ```json
    {
      "message": "Original content and associated cards deleted successfully."
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized, 404 Not Found

### 2.3. Cards

#### Create Card
- **Method:** POST  
- **URL:** `/cards`  
- **Description:** Save a new flashcard.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Request Payload:**
    ```json
    {
      "front": "Question text...",
      "back": "Answer text...",
      "generated_by": "human",
      "original_content_id": "1"      
    }
    ```
- **Field Validations:**
    - `front`: Required, 1-1000 characters
    - `back`: Required, 1-5000 characters
    - `generated_by`: Required, must be either "AI" or "human"
    - `original_content_id`: Optional for manual cards, required for AI-generated cards    
- **Response:**
    ```json
    {
      "id": 101,
      "user_id": 1,
      "front": "Question text...",
      "back": "Answer text...",
      "generated_by": "human",
      "original_content_id": "1",
      "created_at": "2025-04-19T12:05:00Z",
      "updated_at": null
    }
    ```
- **Success Codes:** 201 Created  
- **Error Codes:** 
    - 400 Bad Request (validation errors)
    - 401 Unauthorized
    - 422 Unprocessable Entity (business rule violations)

#### Generate Card using AI
- **Method:** POST  
- **URL:** `/cards/generate`  
- **Description:** Generates a flashcard using AI based on referenced original content.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Request Payload:**
    ```json
    {
      "original_content_id": "1"
    }
    ```
- **Response:**
    ```json
    {
      "id": 102,
      "user_id": 1,
      "original_content_id": "1",
      "front": "Generated summary question...",
      "back": "Generated detailed answer...",
      "generated_by": "AI",
      "created_at": "2025-04-19T12:10:00Z"
    }
    ```
- **Success Codes:** 201 Created  
- **Error Codes:** 400 Bad Request, 401 Unauthorized, 404 Not Found (if original content doesn't exist), 422 Unprocessable Entity (if AI generation fails)

#### Get Cards List
- **Method:** GET  
- **URL:** `/cards`  
- **Description:** Retrieves a paginated list of the authenticated user's cards. Supports filtering and sorting.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Query Parameters:**
    - `page` (optional, default: 1)
    - `limit` (optional, default: 20)
    - `sort` (optional, e.g., `created_at_desc`)
    - `generated_by` (optional, e.g., `AI` or `human`)
- **Response:**
    ```json
    {
      "cards": [
        {
          "id": 102,
          "user_id": 1,
          "front": "Generated summary question...",
          "back": "Generated detailed answer...",
          "generated_by": "AI",
          "created_at": "2025-04-19T12:10:00Z"
        }
      ],
      "pagination": {
        "page": 1,
        "limit": 20,
        "total": 35
      }
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized

#### Get Single Card
- **Method:** GET  
- **URL:** `/cards/{id}`  
- **Description:** Retrieves a specific card by its ID.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Response:**
    ```json
    {
      "id": 102,
      "user_id": 1,
      "original_content_id": "1",
      "front": "Generated summary question...",
      "back": "Generated detailed answer...",
      "generated_by": "AI",
      "created_at": "2025-04-19T12:10:00Z"
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized, 404 Not Found

#### Update Card
- **Method:** PUT  
- **URL:** `/cards/{id}`  
- **Description:** Updates an existing card. Applies for manual edits or modifications after AI generation.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Request Payload:**
    ```json
    {
      "front": "Updated question text...",
      "back": "Updated answer text..."
    }
    ```
- **Response:**
    ```json
    {
      "id": 102,
      "user_id": 1,
      "front": "Updated question text...",
      "back": "Updated answer text...",
      "generated_by": "AI",
      "created_at": "2025-04-19T12:10:00Z",
      "updated_at": "2025-04-19T12:20:00Z"
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 400 Bad Request, 401 Unauthorized, 404 Not Found

#### Delete Card
- **Method:** DELETE  
- **URL:** `/cards/{id}`  
- **Description:** Deletes the specified card.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Response:**
    ```json
    {
      "message": "Card deleted successfully."
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized, 404 Not Found

#### Retrieve Card Error Logs
- **Method:** GET  
- **URL:** `/cards/{id}/errors`  
- **Description:** Retrieves error logs related to a specific card.  
- **Headers:** `Authorization: Bearer JWT_TOKEN_HERE`  
- **Response:**
    ```json
    {
      "card_id": 102,
      "errors": [
        {
          "id": 15,
          "error_details": "Detailed error information...",
          "logged_at": "2025-04-19T12:15:00Z"
        }
      ]
    }
    ```
- **Success Codes:** 200 OK  
- **Error Codes:** 401 Unauthorized, 404 Not Found

## 3. Authentication and Authorization

- **Mechanism:** JWT-based authentication.  
- **Implementation Details:**  
  - Endpoints require the header `Authorization: Bearer <token>`.  
  - Tokens are issued after successful login and must be validated for each request.  
  - The API uses the authenticated user’s ID for row-level security in the database, ensuring users only access their own cards.
- **Additional Considerations:**  
  - Rate limiting and account lockout mechanisms can be enforced to protect against brute-force attacks.

## 4. Validation and Business Logic

- **Validation:**  
  - **Users:** Ensure email uniqueness and enforce required fields for registration.
  - **Cards:**  
    - Validate that `original_content_id` references an existing content entry when provided
    - Validate `front` (1–1000 characters) and `back` (1–5000 characters) on all card creation and updates.  
    - Check that `generated_by` is either `"AI"` or `"human"`.
  - **Error Logs:** Ensure association with a valid card ID.

- **Business Logic Implementation:**  
  - **AI Card Generation:**  
    - Endpoint `/cards/generate` accepts an original content ID; backend retrieves the content and calls the AI service (such as Openrouter.ai) to generate flashcard content using summarization and content-editing techniques.  
    - The generated card data is then validated against business rules before being saved.
  - **Manual Card Creation:**  
    - Users can create or edit cards; updates are saved immediately and reflect user modifications.
  - **Pagination, Filtering, and Sorting:**  
    - List endpoints (e.g., `/cards`) implement pagination by accepting `page` and `limit` query parameters, along with optional filters (such as `generated_by`) and sorting options.
  - **Row Level Security (RLS):**  
    - The API sets the current user's ID (e.g., via request context) to ensure the database applies RLS policies when querying the `cards` table.

- **Error Handling:**  
  - Use early returns and guard clauses to manage unexpected conditions.  
  - Return clear error messages and HTTP status codes (e.g., 400 for bad requests, 401 for unauthorized access, 404 when resources are not found).
