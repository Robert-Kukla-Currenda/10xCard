# API Endpoint Implementation Plan: Get Original Contents List

## 1. Przegląd punktu końcowego
Endpoint ma za zadanie pobierać spersonalizowaną, stronicowaną listę oryginalnych treści użytkownika. Użytkownik musi być uwierzytelniony za pomocą tokenu JWT przesyłanego w nagłówku żądania.

## 2. Szczegóły żądania
- **Metoda HTTP:** GET  
- **Struktura URL:** `/original-contents`  
- **Nagłówki:**  
  - `Authorization: Bearer JWT_TOKEN_HERE`  
- **Parametry zapytania:**  
  - **Opcjonalne:**  
    - `page` (domyślnie: 1) – numer strony (oczekiwany typ: liczba całkowita)  
    - `limit` (domyślnie: 20) – liczba elementów na stronę (oczekiwany typ: liczba całkowita)  
    - `sort` – sposób sortowania (np. `created_at_desc`)

## 3. Wykorzystywane typy
- **DTOs:**  
  - `OriginalContentDto` (odzwierciedlający strukturę oryginalnej treści: `id`, `user_id`, `content`, `created_at`)  
- **Komponenty walidujące:**  
  - Walidacja query parameters (sprawdzenie, czy `page` i `limit` są liczbami oraz czy `sort` spełnia przyjęty format)

## 4. Szczegóły odpowiedzi
- **Struktura odpowiedzi (JSON):**
  ```json
  {
    "items": [
      {
        "id": 1,
        "user_id": 1,
        "content": "Long text content...",
        "created_at": "2025-04-19T12:00:00Z"
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 20,
      "total": 35
    }
  }
  ```
- **Kody statusu:**  
  - 200 OK – przy pomyślnym odczycie  
  - 401 Unauthorized – przy niepoprawnym lub brakującym tokenie

## 5. Przepływ danych
1. Odbiór żądania wraz z tokenem JWT i opcjonalnymi parametrami zapytania.
2. Weryfikacja tokenu przez middleware autoryzacyjny.
3. Walidacja parametrów `page`, `limit` (oraz `sort` jeśli podany).
4. Zapytanie do bazy danych – tabela `original_contents` – filtrowanie po `user_id` odpowiadającym uwierzytelnionemu użytkownikowi, zastosowanie paginacji oraz sortowania.
5. Zebranie wyników oraz wyliczenie całkowitej liczby rekordów dla stworzenia obiektu paginacji.
6. Odesłanie odpowiedzi w formacie JSON zgodnym z powyższą strukturą.

## 6. Względy bezpieczeństwa
- **Uwierzytelnianie i Autoryzacja:**  
  - Sprawdzenie ważności tokenu JWT w nagłówku `Authorization`.  
  - Upewnienie się, że użytkownik otrzymuje tylko swoje treści.  
- **Walidacja wejścia:**  
  - Dokładne sprawdzanie i sanitizacja parametrów `page`, `limit` oraz `sort`.  
- **Baza Danych:**  
  - Użycie zapytań przygotowanych (prepared statements) dla ochrony przed SQL Injection.
- **Dodatkowe:**  
  - Logowanie prób dostępu z nieprawidłowym tokenem lub niewłaściwymi parametrami.

## 7. Obsługa błędów
- **401 Unauthorized:**  
  - Brak lub niewłaściwy token JWT.
- **400 Bad Request:**  
  - Nieprawidłowe parametry zapytania (np. `page` lub `limit` nie są liczbami albo `sort` ma błędny format).
- **500 Internal Server Error:**  
  - Błąd po stronie serwera lub bazy danych.
- **Mechanizmy logowania:**  
  - Rejestrowanie błędów w dedykowanym systemie logowania (ewentualnie w tabeli błędów, jeśli taka logika została już zaimplementowana).

## 8. Rozważania dotyczące wydajności
- **Paginacja:**  
  - Ograniczenie liczby rekordów na stronie (domyślny limit 20) aby zapobiec przeciążeniu serwera.
- **Indeksy w bazie:**  
  - Upewnienie się, że kolumny `user_id` oraz `created_at` mają odpowiednie indeksy wspierające filtrowanie i sortowanie.
- **Ewentualne cache’owanie:**  
  - Rozważenie cache’owania wyników zapytań dla przyspieszenia kolejnych żądań od tego samego użytkownika.

## 9. Etapy wdrożenia
1. **Implementacja Middleware Autoryzacji:**  
   - Uzupełnienie lub weryfikacja istniejącego middleware, który sprawdza nagłówek `Authorization`.
2. **Walidacja Parametrów Zapytania:**  
   - Dodanie mechanizmu walidacji query parameters (`page`, `limit`, `sort`) zgodnie z wymaganiami.
3. **Warstwa Dostępu do Danych:**  
   - Implementacja logiki pobierania danych z tabeli `original_contents` – filtrowanie po `user_id` i zastosowanie paginacji oraz sortowania.
   - Rozważenie wydzielenia logiki do dedykowanego serwisu (np. `OriginalContentService` w `src/lib/services`).
4. **Tworzenie DTO:**  
   - Zastosowanie istniejącego `OriginalContentDto` lub utworzenie nowego DTO odpowiadającego strukturze odpowiedzi.
5. **Budowanie Odpowiedzi:**  
   - Złożenie odpowiedzi w formacie JSON zawierającym listę elementów oraz dane paginacji.
6. **Obsługa Błędów i Logowanie:**  
   - Implementacja mechanizmu zwracającego odpowiednie kody statusu w zależności od typu błędu.
7. **Testy:**  
   - Napisanie testów jednostkowych oraz integracyjnych dla endpointu, weryfikujących prawidłowy przepływ danych, walidację oraz obsługę błędów.
8. **Dokumentacja:**  
   - Uaktualnienie dokumentacji API z nowym endpointem i jego specyfikacją.

