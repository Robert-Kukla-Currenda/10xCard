# API Endpoint Implementation Plan: Get Cards List

## 1. Przegląd punktu końcowego
- **Cel:** Umożliwić uwierzytelnionemu użytkownikowi pobranie listy fiszek z możliwością paginacji, filtrowania po typie generacji (AI lub human) oraz sortowania (np. `created_at_desc`).
- **Funkcjonalność:** Endpoint ma zwracać zestaw fiszek użytkownika wraz z danymi paginacyjnymi.

## 2. Szczegóły żądania
- **Metoda HTTP:** GET
- **URL:** `/cards`
- **Nagłówki:** 
  - `Authorization: Bearer JWT_TOKEN_HERE` (wymagane)
- **Parametry zapytania:**
  - **Wymagane:** Brak (poza autoryzacją)
  - **Opcjonalne:**
    - `page` (domyślnie 1)
    - `limit` (domyślnie 20)
    - `sort` (np. `created_at_desc`)
    - `generated_by` (np. `AI` lub `human`)
- **Request Body:** Brak

## 3. Wykorzystywane typy
- **DTOs:**
  - `CardDto` – reprezentuje pojedynczą fiszkę
- **Command Models:** 
  - Nie dotyczy, gdyż jest to żądanie GET, więc nie są wysyłane żadne dane do stworzenia lub modyfikacji.

## 4. Szczegóły odpowiedzi
- **Struktura odpowiedzi (przykład):**
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
- **Kody statusu:**
  - `200 OK` – powodzenie odczytu
  - `401 Unauthorized` – nieprawidłowy lub brak tokenu

## 5. Przepływ danych
1. **Odbiór żądania:** Endpoint odbiera żądanie GET z nagłówkiem autoryzacyjnym JWT oraz opcjonalnymi parametrami paginacyjnymi, filtrowania i sortowania.
2. **Weryfikacja użytkownika:** Na podstawie tokenu JWT identyfikowany jest użytkownik.
3. **Wywołanie warstwy serwisowej:** Przekazanie identyfikatora użytkownika wraz z parametrami zapytania do metody serwisowej (np. `CardService.getPaginatedCards`).
4. **Pozyskanie danych:** Serwis pobiera dane z tabeli `cards`, stosując politykę RLS, aby zwrócić tylko fiszki użytkownika.
5. **Mapowanie danych:** Dane są mapowane do struktury `CardDto`.
6. **Zwrócenie odpowiedzi:** Endpoint zwraca dane fiszek wraz z informacjami paginacyjnymi.

## 6. Względy bezpieczeństwa
- **Uwierzytelnianie:** Weryfikacja tokenu JWT, zabezpieczenie endpointu przed dostępem nieautoryzowanych użytkowników.
- **RLS (Row Level Security):** Baza danych musi zostać skonfigurowana, aby użytkownik widział tylko swoje fiszki.
- **Walidacja parametrów:** Weryfikacja poprawności parametrów zapytania – np. czy `page` i `limit` są liczbami, czy `generated_by` przyjmuje poprawne wartości.

## 7. Obsługa błędów
- **Brak lub nieważny token:** Zwrócić `401 Unauthorized`.
- **Nieprawidłowe parametry (np. niepoprawny format):** Zwrócić `400 Bad Request` z odpowiednimi komunikatami.
- **Błąd pobierania danych:** W sytuacji błędów serwera zwrócić `500 Internal Server Error`.
- **Rejestracja błędów:** Logowanie błędów, które mogą być związane również z tabelą błędów (jeśli występuje potrzeba rejestracji specyficznych błędów związanych z fiszkami).

## 8. Rozważania dotyczące wydajności
- **Paginacja:** Użycie paginacji ogranicza zwracane dane i zmniejsza obciążenie serwera.
- **Indeksowanie:** Upewnić się, że kluczowe kolumny (np. `created_at`, `user_id`, `generated_by`) są indeksowane, aby zoptymalizować zapytania.
- **Optymalizacja zapytań:** Minimalizacja dodatkowych operacji na bazie danych oraz stosowanie właściwych zapytań dla paginacji i filtrowania.

## 9. Etapy wdrożenia
1. **Utworzenie endpointu API:**
   - Utworzenie pliku (np. `src/pages/api/cards/index.ts`) z obsługą żądania GET.
2. **Walidacja tokenu:**
   - Integracja z systemem uwierzytelniania i weryfikacja nagłówka `Authorization`.
3. **Parsowanie i walidacja parametrów zapytania:**
   - Sprawdzenie obecności i poprawności parametrów `page`, `limit`, `sort` i `generated_by`.
4. **Implementacja logiki serwisowej:**
   - Utworzenie lub aktualizacja serwisu (np. `src/lib/services/CardService.ts`) do pobierania fiszek z bazy danych przy użyciu warstwy ORM lub bezpośrednich zapytań.
5. **Mapowanie danych do DTO:**
   - Mapowanie wyników z bazy danych do typu `CardDto`.
6. **Obsługa odpowiedzi i błędów:**
   - Przygotowanie poprawnej struktury odpowiedzi oraz obsługa możliwych błędów z odpowiednimi kodami statusu.
7. **Testowanie:**
   - Implementacja testów jednostkowych i integracyjnych dla endpointu.
8. **Wdrożenie i monitorowanie:**
   - Deploy na środowisko testowe i monitorowanie logów w celu identyfikacji ewentualnych problemów.
