# API Endpoint Implementation Plan: Create Original Content

## 1. Przegląd punktu końcowego
Endpoint służy do zapisywania nowej treści oryginalnej, która będzie używana do generowania fiszek. Dostęp do punktu końcowego mają tylko uwierzytelnieni użytkownicy.

## 2. Szczegóły żądania
- **Metoda HTTP:** POST  
- **Struktura URL:** `/original-contents`
- **Parametry:**
  - **Wymagane:**  
    - Nagłówek `Authorization: Bearer JWT_TOKEN_HERE`
    - Body:  
      - `content` (string) – tekst o długości od 1000 do 10000 znaków  
  - **Opcjonalne:** Brak dodatkowych parametrów
- **Request Body Example:**
  ```json
  {
    "content": "Long text content that will be used to generate flashcards..."
  }
  ```

## 3. Wykorzystywane typy
- **Command Model:** `CreateOriginalContentCommand` (definiowany w Commands.cs), który zawiera walidacje dla pola `content`.
- **DTO:** `OriginalContentDto` (definiowany w DTOs.cs) odpowiedzialny za strukturę zwracaną w odpowiedzi.

## 4. Szczegóły odpowiedzi
- **Przykładowa odpowiedź (Status 201 Created):**
  ```json
  {
    "id": 1,
    "user_id": 1,
    "content": "Long text content that will be used to generate flashcards...",
    "created_at": "2025-04-19T12:00:00Z"
  }
  ```
- **Kody statusu:**
  - 201 Created – gdy treść zostanie poprawnie zapisana
  - 400 Bad Request – gdy wystąpią błędy walidacji (np. niepoprawna długość `content`)
  - 401 Unauthorized – gdy żądanie nie posiada poprawnego tokena

## 5. Przepływ danych
1. Uwierzytelnienie użytkownika przez sprawdzenie tokena JWT w nagłówku.
2. Odbiór oraz walidacja danych wejściowych przy użyciu `CreateOriginalContentCommand`.
3. Przekazanie danych do warstwy serwisu (`OriginalContentService`) odpowiedzialnej za logikę biznesową.
4. Serwis zapisuje dane do tabeli `original_contents` powiązanej relacją z tabelą `users`.
5. Po zapisaniu danych, zwracana jest struktura `OriginalContentDto` z informacjami o nowo utworzonej treści.

## 6. Względy bezpieczeństwa
- Weryfikacja nagłówka `Authorization` i tokena JWT w middleware przed dotarciem do logiki endpointa.
- Potwierdzenie, że użytkownik posiada stosowne uprawnienia do tworzenia treści.
- Unikanie wycieku informacji poprzez szczegółowe komunikaty błędów.
- Sprawdzenie, czy przekazywany tekst spełnia wymogi długości i nie zawiera potencjalnie szkodliwych znaków.

## 7. Obsługa błędów
- **400 Bad Request:** Gdy:
  - Długość pola `content` nie mieści się w dozwolonym zakresie.
  - Inne błędy walidacji wynikające z atrybutów walidacyjnych.
- **401 Unauthorized:** Gdy:
  - Token JWT jest nieobecny lub nieważny.
- **500 Internal Server Error:** W przypadku niespodziewanych błędów podczas przetwarzania, np. problemy z połączeniem z bazą.
- (Opcjonalnie) Rejestrowanie błędów wykorzystując mechanizm logowania, potencjalnie zapisując szczegóły błędów przy użyciu `ErrorLogDto`.

## 8. Rozważania dotyczące wydajności
- Optymalizacja zapytania insertu dzięki wykorzystaniu mechanizmów ORM (Entity Framework lub innego stosowanego narzędzia).
- Upewnienie się, że walidacja odbywa się przed próbą zapisu danych do bazy, aby unikać zbędnych operacji.
- Implementacja paginacji i indeksów w bazie danych w celu obsługi rosnącej liczby treści oryginalnych (dotyczy endpointu pobierającego listę).

## 9. Etapy wdrożenia
1. **Implementacja uwierzytelniania:**
   - Sprawdzić middleware odpowiedzialne za weryfikację tokena JWT.
2. **Stworzenie modelu Command i DTO:**
   - Upewnić się, że `CreateOriginalContentCommand` i `OriginalContentDto` są poprawnie zdefiniowane.
3. **Zaimplementowanie Endpointa:**
   - Utworzyć nową metodę w kontrolerze, która obsługuje metodę POST na `/original-contents`.
4. **Wyodrębnienie logiki biznesowej:**
   - Utworzyć serwis `OriginalContentService` w katalogu `src/lib/services`, który odpowiada za walidację i zapis danych.
5. **Integracja z bazą danych:**
   - Zaimplementować logikę zapisu do tabeli `original_contents` i obsłużyć relację z tabelą `users`.
6. **Testy:**
   - Stworzyć testy jednostkowe oraz integracyjne, sprawdzające poprawność walidacji, zapisu i odpowiedzi API.
7. **Obsługa błędów i logowanie:**
   - Dodać mechanizm logowania przy błędach i zapewnić zwracanie właściwych kodów statusu.
8. **Review i wdrożenie:**
   - Przeprowadzić code review oraz testy bezpieczeństwa przed wdrożeniem do środowiska produkcyjnego.

