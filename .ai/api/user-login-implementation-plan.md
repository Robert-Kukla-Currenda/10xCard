# API Endpoint Implementation Plan: User Login

## 1. Przegląd punktu końcowego
Endpoint umożliwia użytkownikowi logowanie poprzez weryfikację poświadczeń (adres email oraz hasło). W przypadku powodzenia generowany jest JWT token, a użytkownik otrzymuje dane umożliwiające dalsze autoryzowane operacje.

## 2. Szczegóły żądania
- **Metoda HTTP:** POST
- **Struktura URL:** `/users/login`
- **Parametry:**
  - **Wymagane (w ciele żądania):**
    - `email` (string): Adres email użytkownika.
    - `password` (string): Hasło użytkownika.
- **Request Body Example:**
  ```json
  {
    "email": "user@example.com",
    "password": "PlainTextPassword"
  }
  ```

## 3. Wykorzystywane typy
- **DTO:**
  - `LoginResultDto` – zawiera właściwości:
    - `Token` (string)
    - `User` (UserDto, który zawiera `id`, `email`, `first_name`, `last_name`, `created_at`)
- **Command Model:**
  - `LoginUserCommand` – zawiera:
    - `Email` (string)
    - `Password` (string)

## 4. Szczegóły odpowiedzi
- **Sukces:**
  - **Kod 200 OK**
  - **Body:**
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
- **Błędy:**
  - **401 Unauthorized:** Gdy podane poświadczenia są niepoprawne.
  - **400 Bad Request:** Gdy walidacja wejściowych danych się nie powiedzie.
  - **500 Internal Server Error:** Dla nieoczekiwanych błędów po stronie serwera.

## 5. Przepływ danych
1. Żądanie logowania trafia do kontrolera odpowiedzialnego za `/users/login`.
2. Dane wejściowe są walidowane przy użyciu `LoginUserCommand` (atrybuty [Required], [EmailAddress]).
3. Kontroler wywołuje logikę autoryzacyjną w dedykowanej usłudze (np. `AuthenticationService`):
   - Pobiera dane użytkownika z bazy danych na podstawie `email`.
   - Weryfikuje hasło poprzez porównanie z przechowywanym `password_hash`.
   - W przypadku sukcesu generowany jest JWT token.
4. Wynik jest mapowany do obiektu `LoginResultDto` i zwracany do klienta.
5. W przypadku błędów, serwis zwraca odpowiedni kod błędu i komunikat.

## 6. Względy bezpieczeństwa
- **Walidacja:** Wykorzystanie atrybutów walidacyjnych w `LoginUserCommand`.
- **Porównanie hasła:** Użycie bezpiecznej metody hash'owania i porównania haseł.
- **Ochrona przed brute force:** Implementacja mechanizmu ograniczającego liczbę prób logowania.
- **JWT:** Generowanie tokena z użyciem silnego sekretu oraz ustawienie limitu ważności.
- **Logowanie błędów:** Rejestrowanie nieudanych prób logowania i błędów serwerowych.

## 7. Obsługa błędów
- **Błąd 400 Bad Request:** Gdy dane wejściowe nie spełniają warunków walidacji.
- **Błąd 401 Unauthorized:** Gdy poświadczenia są nieprawidłowe.
- **Błąd 500 Internal Server Error:** W przypadku niespodziewanych błędów systemowych.
- Szczegółowe komunikaty błędów powinny być logowane, ale nie ujawniane w odpowiedzi dla bezpieczeństwa.

## 8. Rozważania dotyczące wydajności
- **Optymalizacja zapytań do bazy:** Indeksowanie pola `email` w tabeli `users`.
- **Asynchroniczność:** Użycie operacji asynchronicznych dla interakcji z bazą danych.
- **Cache:** Rozważyć cache'owanie konfiguracji JWT (np. sekret, czas ważności) jeśli to potrzebne.

## 9. Etapy wdrożenia
1. **Przygotowanie środowiska:**
   - Upewnić się, że tabela `users` jest poprawnie skonfigurowana.
   - Skonfigurować parametry JWT (sekret, czas ważności).
2. **Implementacja walidacji:**
   - Użycie atrybutów walidacyjnych w `LoginUserCommand` do weryfikacji struktury danych wejściowych.
3. **Implementacja serwisu autoryzacji:**
   - Utworzyć lub rozszerzyć `AuthenticationService` dodając metodę, która:
     - Pobiera użytkownika na podstawie adresu email.
     - Weryfikuje hasło.
     - Generuje i zwraca JWT token.
4. **Implementacja endpointu:**
   - Utworzyć kontroler endpointu `/users/login`.
   - Zaimplementować logikę, która:
     - Waliduje dane wejściowe.
     - Wywołuje metodę serwisu autoryzacji.
     - Mapuje wynik do `LoginResultDto` i zwraca odpowiedź.
5. **Testy jednostkowe i integracyjne:**
   - Opracować testy sprawdzające:
     - Poprawne logowanie (kod 200 i struktura odpowiedzi).
     - Błędne dane logowania (kod 401).
     - Błędne formaty danych wejściowych (kod 400).
6. **Logowanie i monitoring błędów:**
   - Dodanie mechanizmu logującego nieudane próby logowania oraz błędy serwerowe.
7. **Code Review i wdrożenie:**
   - Przeprowadzić code review przez zespół.
   - Wdrożyć endpoint w środowisku testowym, a następnie na produkcyjnym.