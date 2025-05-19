# API Endpoint Implementation Plan: Get Single Original Content

## 1. Przegląd punktu końcowego
Endpoint umożliwia pobranie pojedynczej treści oryginalnej na podstawie jej identyfikatora. Wymaga autoryzacji poprzez przekazanie ważnego tokena JWT w nagłówku. Endpoint zapewnia dostęp tylko uprawnionym użytkownikom oraz odpowiada odpowiednimi kodami statusu w zależności od rezultatu operacji.

## 2. Szczegóły żądania
- **Metoda HTTP:** GET
- **Struktura URL:** `/original-contents/{id}`
- **Parametry:**
  - **Wymagane:**
    - `id` (ścieżkowy parametr): unikalny identyfikator treści oryginalnej (typ: liczba całkowita)
    - Nagłówek `Authorization`: wymagany w formacie `Bearer JWT_TOKEN_HERE`
  - **Opcjonalne:** Brak
- **Request Body:** Brak

## 3. Wykorzystywane typy
- **DTO:**
  - `OriginalContentDto` – reprezentuje dane treści oryginalnej
- **Command Model:** Brak, ponieważ endpoint tylko odczytuje dane

## 4. Szczegóły odpowiedzi
- **Sukces:**
  - **Kod statusu:** 200 OK
  - **Przykładowa odpowiedź:**
    ```json
    {
      "id": 1,
      "user_id": 1,
      "content": "Long text content...",
      "created_at": "2025-04-19T12:00:00Z"
    }
    ```
- **Błędy:**
  - **401 Unauthorized:** Brak lub nieprawidłowy token JWT
  - **404 Not Found:** Nie znaleziono treści oryginalnej o podanym identyfikatorze
  - **500 Internal Server Error:** Błąd po stronie serwera

## 5. Przepływ danych
1. **Przyjęcie żądania:** Odbiór żądania GET z podanym identyfikatorem oraz tokenem JWT w nagłówku.
2. **Weryfikacja autoryzacji:** Sprawdzenie poprawności tokena JWT.
3. **Walidacja identyfikatora:** Sprawdzenie, czy przekazany parametr `id` jest prawidłowym numerem.
4. **Pobranie danych:** Wykonanie zapytania do bazy danych (tabela `original_contents`) w celu odczytania rekordu o zadanym `id`.
5. **Mapowanie danych:** Konwersja danych z bazy na obiekt `OriginalContentDto`.
6. **Odpowiedź:** Wysłanie odpowiedzi z kodem 200 i danymi w formacie JSON lub odpowiedniego błędu (401 lub 404).

## 6. Względy bezpieczeństwa
- **Autoryzacja:** Walidacja tokena JWT w nagłówku `Authorization`.
- **Walidacja danych:** Upewnienie się, że przekazany parametr `id` jest poprawnym typem liczbowym.
- **Dostęp do danych:** Sprawdzenie, czy wywołujący ma uprawnienia do dostępu do danej treści (ewentualna weryfikacja powiązania treści z użytkownikiem).
- **Ochrona przed atakami:** Zapobieganie SQL Injection i innym typowym zagrożeniom poprzez stosowanie bezpiecznych zapytań i ORM.

## 7. Obsługa błędów
- **401 Unauthorized:** Gdy token JWT jest nieobecny lub niepoprawny.
- **404 Not Found:** Gdy brak rekordu z podanym `id` w bazie danych.
- **500 Internal Server Error:** Dla nieoczekiwanych wyjątków lub błędów po stronie serwera.
- Wszystkie błędy należy odpowiednio logować, stosując mechanizmy error loggingu, aby ułatwić debugowanie oraz monitoring działania systemu.

## 8. Rozważania dotyczące wydajności
- **Optymalizacja zapytań:** Upewnienie się, że tabela `original_contents` posiada indeks na kolumnie `id`, co przyspieszy wyszukiwanie.
- **Caching:** W zależności od obciążenia można rozważyć mechanizmy cache’owania wyników zapytań.
- **Monitorowanie:** Ustalanie limitów czasowych zapytań oraz monitorowanie wydajności endpointu.

## 9. Etapy wdrożenia
1. **Przygotowanie infrastruktury:**
   - Skonfigurowanie bazy danych i upewnienie się o istnieniu indeksów na tabeli `original_contents`.
   - Upewnienie się o działaniu mechanizmu weryfikacji JWT.
2. **Implementacja logiki biznesowej:**
   - Wdrożenie logiki pobierania treści w warstwie serwisowej (np. `OriginalContentService`), która odpowiada za walidację tokena, weryfikację parametru `id` oraz interakcję z bazą danych.
3. **Implementacja kontrolera:**
   - Utworzenie endpointu w warstwie API, który przyjmie żądanie, wywoła metodę serwisową i zwróci wynik jako obiekt `OriginalContentDto` lub odpowiedni błąd.
4. **Dodanie obsługi błędów:**
   - Zaimplementowanie mechanizmu obsługi wyjątków, który przechwytuje problemy związane z autoryzacją, brakiem danych oraz innymi błędami.
5. **Testy:**
   - Implementacja testów jednostkowych oraz integracyjnych w celu weryfikacji poprawności działania endpointu.
   - Testowanie scenariuszy: poprawne pobranie danych, próba pobrania nieistniejącego rekordu, żądanie bez poprawnego tokena.
6. **Walidacja i review:**
   - Przeprowadzenie code review oraz testów bezpieczeństwa.
7. **Wdrożenie na środowisko staging oraz produkcyjne:**
   - Monitorowanie endpointu pod kątem wydajności i błędów oraz wprowadzenie ewentualnych optymalizacji.
