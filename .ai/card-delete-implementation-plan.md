# API Endpoint Implementation Plan: Delete Card

## 1. Przegląd punktu końcowego
Endpoint umożliwia usunięcie fiszki o podanym identyfikatorze (id). Działanie endpointu ograniczone jest tylko do właściciela fiszki, dzięki dedykowanym regułom RLS. Uwierzytelnianie odbywa się przez token JWT przekazany w nagłówku żądania.

## 2. Szczegóły żądania
- **Metoda HTTP:** DELETE  
- **Struktura URL:** `/cards/{id}`  
- **Parametry:**  
  - **Wymagane:**  
    - `{id}` – identyfikator karty do usunięcia  
  - **Nagłówki:**  
    - `Authorization: Bearer JWT_TOKEN_HERE`
- **Request Body:** Brak

## 3. Wykorzystywane typy
- **DTOs:**  
  - `CardDto` – reprezentuje zasób karty, użyte przy ewentualnych operacjach odczytu lub walidacji.
- **Command Modele:**  
  - Nie stosujemy dedykowanego Command Model dla usunięcia.

## 4. Szczegóły odpowiedzi
- **Sukces (200 OK):**  
  ```json
  {
    "message": "Card deleted successfully."
  }
  ```
- **Błędy:**  
  - **401 Unauthorized:** Gdy token JWT jest niepoprawny lub wygasł.
  - **404 Not Found:** Gdy karta o podanym id nie istnieje albo nie należy do zalogowanego użytkownika.
  - **500 Internal Server Error:** Jeśli wystąpią nieoczekiwane błędy serwera.

## 5. Przepływ danych
1. Żądanie przychodzi na endpoint `/cards/{id}` z poprawnym tokenem JWT w nagłówku.
2. Middleware uwierzytelniający waliduje token i ustawia dane użytkownika w kontekście.
3. Serwis `CardService` pobiera identyfikator karty oraz identyfikator użytkownika z kontekstu.
4. Serwis sprawdza istnienie karty i jej przynależność do użytkownika (zgodnie z RLS).
5. Jeśli karta została odnaleziona i uprawnienia są poprawne, wykonywane jest zapytanie usuwające kartę.
6. Na zakończenie zwracany jest komunikat o pomyślnym usunięciu.

## 6. Względy bezpieczeństwa
- **Uwierzytelnianie:** Weryfikacja tokena JWT z wykorzystaniem odpowiedniej biblioteki.
- **Autoryzacja:** Sprawdzenie, czy usuwana karta należy do osoby wykonującej operację, zgodnie z polityką RLS.
- **Walidacja:** Sprawdzenie poprawności danych wejściowych, tj. czy id jest poprawnym identyfikatorem liczbowym.

## 7. Obsługa błędów
- **401 Unauthorized:** Brak lub niepoprawny nagłówek `Authorization`.  
  - Zwrócenie komunikatu o błędzie uwierzytelnienia.
- **404 Not Found:** Karta nie została znaleziona lub nie należy do zalogowanego użytkownika.  
  - Zwrócenie komunikatu informującego o braku takiej karty.
- **500 Internal Server Error:** Niespodziewane błędy podczas przetwarzania żądania.  
  - Logowanie błędów oraz zwrócenie ogólnego komunikatu o błędzie.

## 8. Rozważania dotyczące wydajności
- Upewnienie się, że zapytania do bazy danych są zoptymalizowane (np. dzięki odpowiednim indeksom na kolumnach `id` i `user_id`).
- Minimalizacja liczby zapytań – wykonanie bezpośredniego zapytania usuwającego, po potwierdzeniu uprawnień użytkownika.
- Rozważenie cache’owania konfiguracji RLS, jeżeli jest to potrzebne w szerszym kontekście.

## 9. Etapy wdrożenia
1. **Uwierzytelnianie i autoryzacja:**  
   - Sprawdzenie i ew. aktualizacja middleware odpowiedzialnego za walidację tokena JWT.
   - Upewnienie się, że kontekst użytkownika jest prawidłowo ustawiany.

2. **Implementacja funkcji w serwisie:**  
   - Utworzenie metody `DeleteCardAsync(int cardId, int userId)` w `CardService`.
   - Dodanie walidacji, czy karta o danym id istnieje i należy do użytkownika.
   - Obsługa zapytań do bazy danych, w tym usunięcie rekordu, wykorzystując polityki RLS.

3. **Aktualizacja kontrolera API:**  
   - Dodanie endpointu DELETE `/cards/{id}`.
   - Wczytanie id ze ścieżki oraz informacji o użytkowniku z kontekstu.
   - Wywołanie odpowiedniej metody z `CardService` i zwrócenie komunikatu potwierdzającego usunięcie.

4. **Testy jednostkowe i integracyjne:**  
   - Utworzenie testów sprawdzających poprawność usuwania karty.
   - Testy scenariuszy błędnych: nieautoryzowane, karta nie istnieje, błędy serwera.

5. **Logowanie i monitorowanie:**  
   - Implementacja logowania błędów w przypadku nieudanych operacji.
   - Monitorowanie żądań oraz logi błędów.

6. **Dokumentacja:**  
   - Zaktualizowanie dokumentacji API, aby uwzględniała nye możliwości endpointu DELETE `/cards/{id}`.

---

Wdrożony plan należy zapisać jako:

**filepath:** c:\Projekty\_szkolenia\10xDevs\.ai\view-implementation-plan.md
# API Endpoint Implementation Plan: Delete Card

## 1. Przegląd punktu końcowego
Endpoint umożliwia usunięcie fiszki o podanym identyfikatorze (id). Działanie endpointu ograniczone jest tylko do właściciela fiszki, dzięki dedykowanym regułom RLS. Uwierzytelnianie odbywa się przez token JWT przekazany w nagłówku żądania.

## 2. Szczegóły żądania
- **Metoda HTTP:** DELETE  
- **Struktura URL:** `/cards/{id}`  
- **Parametry:**  
  - **Wymagane:**  
    - `{id}` – identyfikator karty do usunięcia  
  - **Nagłówki:**  
    - `Authorization: Bearer JWT_TOKEN_HERE`
- **Request Body:** Brak

## 3 Wykorzystywane typy
- **DTOs:**  
  - `CardDto` – reprezentuje zasób karty, użyte przy ewentualnych operacjach odczytu lub walidacji.
- **Command Modele:**  
  - Nie stosujemy dedykowanego Command Model dla usunięcia.

## 4. Szczegóły odpowiedzi
- **Sukces (200 OK):**  
  ```json
  {
    "message": "Card deleted successfully."
  }
  ```
- **Błędy:**  
  - **401 Unauthorized:** Gdy token JWT jest niepoprawny lub wygasł.
  - **404 Not Found:** Gdy karta o podanym id nie istnieje albo nie należy do zalogowanego użytkownika.
  - **500 Internal Server Error:** Jeśli wystąpią nieoczekiwane błędy serwera.

## 5. Przepływ danych
1. Żądanie przychodzi na endpoint `/cards/{id}` z poprawnym tokenem JWT w nagłówku.
2. Middleware uwierzytelniający waliduje token i ustawia dane użytkownika w kontekście.
3. Serwis `CardService` pobiera identyfikator karty oraz identyfikator użytkownika z kontekstu.
4. Serwis sprawdza istnienie karty i jej przynależność do użytkownika (zgodnie z RLS).
5. Jeśli karta została odnaleziona i uprawnienia są poprawne, wykonywane jest zapytanie usuwające kartę.
6. Na zakończenie zwracany jest komunikat o pomyślnym usunięciu.

## 6. Względy bezpieczeństwa
- **Uwierzytelnianie:** Weryfikacja tokena JWT z wykorzystaniem odpowiedniej biblioteki.
- **Autoryzacja:** Sprawdzenie, czy usuwana karta należy do osoby wykonującej operację, zgodnie z polityką RLS.
- **Walidacja:** Sprawdzenie poprawności danych wejściowych, tj. czy id jest poprawnym identyfikatorem liczbowym.

## 7. Obsługa błędów
- **401 Unauthorized:** Brak lub niepoprawny nagłówek `Authorization`.  
  - Zwrócenie komunikatu o błędzie uwierzytelnienia.
- **404 Not Found:** Karta nie została znaleziona lub nie należy do zalogowanego użytkownika.  
  - Zwrócenie komunikatu informującego o braku takiej karty.
- **500 Internal Server Error:** Niespodziewane błędy podczas przetwarzania żądania.  
  - Logowanie błędów oraz zwrócenie ogólnego komunikatu o błędzie.

## 8. Rozważania dotyczące wydajności
- Upewnienie się, że zapytania do bazy danych są zoptymalizowane (np. dzięki odpowiednim indeksom na kolumnach `id` i `user_id`).
- Minimalizacja liczby zapytań – wykonanie bezpośredniego zapytania usuwającego, po potwierdzeniu uprawnień użytkownika.
- Rozważenie cache’owania konfiguracji RLS, jeżeli jest to potrzebne w szerszym kontekście.

## 9. Etapy wdrożenia
1. **Uwierzytelnianie i autoryzacja:**  
   - Sprawdzenie i ew. aktualizacja middleware odpowiedzialnego za walidację tokena JWT.
   - Upewnienie się, że kontekst użytkownika jest prawidłowo ustawiany.

2. **Implementacja funkcji w serwisie:**  
   - Utworzenie metody `DeleteCardAsync(int cardId, int userId)` w `CardService`.
   - Dodanie walidacji, czy karta o danym id istnieje i należy do użytkownika.
   - Obsługa zapytań do bazy danych, w tym usunięcie rekordu, wykorzystując polityki RLS.

3. **Aktualizacja kontrolera API:**  
   - Dodanie endpointu DELETE `/cards/{id}`.
   - Wczytanie id ze ścieżki oraz informacji o użytkowniku z kontekstu.
   - Wywołanie odpowiedniej metody z `CardService` i zwrócenie komunikatu potwierdzającego usunięcie.

4. **Testy jednostkowe i integracyjne:**  
   - Utworzenie testów sprawdzających poprawność usuwania karty.
   - Testy scenariuszy błędnych: nieautoryzowane, karta nie istnieje, błędy serwera.

5. **Logowanie i monitorowanie:**  
   - Implementacja logowania błędów w przypadku nieudanych operacji.
   - Monitorowanie żądań oraz logi błędów.

6. **Dokumentacja:**  
   - Zaktualizowanie dokumentacji API, aby uwzględniała nye możliwości endpointu DELETE `/cards/{id}`.

