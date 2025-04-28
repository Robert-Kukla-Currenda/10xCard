# API Endpoint Implementation Plan: Get Current User Profile

## 1. Przegląd punktu końcowego
Endpoint GET `/users/me` służy do pobrania profilu uwierzytelnionego użytkownika. Uwierzytelnienie odbywa się przy użyciu tokenu JWT przekazanego w nagłówku `Authorization`.

## 2. Szczegóły żądania
- **Metoda HTTP:** GET  
- **Struktura URL:** `/users/me`  
- **Parametry:**
  - **Wymagane:**  
    - Nagłówek `Authorization` z wartością `Bearer JWT_TOKEN_HERE`
  - **Opcjonalne:**  
    - Brak

## 3. Wykorzystywane typy
- **DTO:**  
  - `UserDto` – zawiera pola: `Id`, `Email`, `FirstName`, `LastName`, `CreatedAt`

## 4. Szczegóły odpowiedzi
- **Kod statusu:**  
  - 200 OK – zwraca profil użytkownika  
  - 401 Unauthorized – brak lub niepoprawny token
- **Struktura odpowiedzi (JSON):**
  ```json
  {
    "id": 1,
    "email": "user@example.com",
    "first_name": "John",
    "last_name": "Doe",
    "created_at": "2025-04-19T12:00:00Z"
  }
  ```

## 5. Przepływ danych
1. Żądanie trafia do endpointu `/users/me` wraz z nagłówkiem `Authorization: Bearer JWT_TOKEN_HERE`.
2. Middleware lub dedykowany serwis weryfikuje i dekoduje token JWT.
3. Na podstawie danych z tokenu (np. id użytkownika) wywoływany jest serwis (np. `UserService`), który pobiera dane użytkownika z tabeli `users`.
4. Dane użytkownika są mapowane do obiektu `UserDto`.
5. Obiekt `UserDto` jest zwracany jako odpowiedź w formacie JSON.

## 6. Względy bezpieczeństwa
- Weryfikacja autentyczności tokenu JWT przed wykonaniem operacji.  
- Upewnienie się, że tylko uwierzytelniony użytkownik może uzyskać dostęp do swoich danych.  
- Ograniczenie ujawniania wrażliwych danych (np. hasła, dane logowania).

## 7. Obsługa błędów
- **401 Unauthorized:**  
  - Zwrot, gdy token jest nieobecny lub niepoprawny.
- **500 Internal Server Error:**  
  - Zwrot, gdy wystąpi nieoczekiwany błąd w trakcie przetwarzania żądania.  
- Wdrożenie centralnego mechanizmu logowania błędów, który rejestruje szczegóły problemów (ewentualnie do tabeli błędów, jeśli istnieje).

## 8. Rozważania dotyczące wydajności
- Minimalny narzut związany z walidacją tokenu oraz prostym zapytaniem do bazy danych.  
- Możliwość implementacji cache’owania profilu użytkownika dla optymalizacji, jeśli profil rzadko się zmienia.

## 9. Etapy wdrożenia
1. **Implementacja Middleware / Walidacji JWT:**  
   - Upewnienie się, że każde żądanie do `/users/me` przechodzi weryfikację tokenu.
2. **Stworzenie lub aktualizacja serwisu odpowiedzialnego za użytkowników (`UserService`):**  
   - Dodanie metody do pobierania profilu użytkownika na podstawie danych z tokenu.
3. **Implementacja Endpointu `/users/me`:**  
   - Odczytanie tokenu, wywołanie serwisu, mapowanie wyników do `UserDto` i zwrócenie odpowiedzi.
4. **Testy jednostkowe i integracyjne:**  
   - Testy z poprawnym i niepoprawnym tokenem.
5. **Implementacja mechanizmu logowania błędów:**  
   - Obsługa wyjątków i rejestracja krytycznych błędów.
6. **Review i wdrożenie:**  
   - Code review, testowanie w środowisku staging, wdrożenie na produkcję.