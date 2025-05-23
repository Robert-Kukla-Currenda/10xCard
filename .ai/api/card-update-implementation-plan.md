# API Endpoint Implementation Plan: Update Card

## 1. Przegląd punktu końcowego
Endpoint służy do aktualizacji istniejącej fiszki. Umożliwia ręczną edycję pól "front" i "back" w fiszce niezależnie od sposobu jej utworzenia (manualnie lub przez AI). Endpoint zapewnia, że użytkownik może modyfikować tylko własne fiszki, zgodnie z polityką RLS.

## 2. Szczegóły żądania
- **Metoda HTTP:** PUT  
- **Struktura URL:** `/cards/{id}`  
- **Parametry:**
  - **Wymagane:**
    - {id} – identyfikator fiszki (przekazany jako parametr ścieżki)
    - Nagłówek `Authorization: Bearer JWT_TOKEN_HERE` – token JWT do uwierzytelnienia
    - Request Body zawierający:
      - `front`: tekst zaktualizowanej części przedniej fiszki (wymagane, długość między 1 a 1000 znaków)
      - `back`: tekst zaktualizowanej części tylnej fiszki (wymagane, długość między 1 a 5000 znaków)

## 3. Wykorzystywane typy
- **DTO:** `CardDto` – model zwracany w odpowiedzi, reprezentujący zaktualizowaną fiszkę.
- **Command Model:** `UpdateCardCommand` – model wejściowy, służący do walidacji danych przekazanych w żądaniu aktualizacji.

## 4. Szczegóły odpowiedzi
- **Kod 200 OK:** W przypadku sukcesu, zwrócony zostanie obiekt typu `CardDto` zawierający:
  - `id` – identyfikator fiszki
  - `user_id` – identyfikator użytkownika właściciela fiszki
  - `front` – zaktualizowany tekst pytania
  - `back` – zaktualizowany tekst odpowiedzi
  - `generated_by` – wskaźnik źródła generacji (np. "AI" lub "human")
  - `created_at` – data utworzenia
  - `updated_at` – data ostatniej aktualizacji

- **Kody błędów:**
  - 400 Bad Request – nieprawidłowe dane wejściowe
  - 401 Unauthorized – brak autoryzacji lub nieważny token
  - 404 Not Found – fiszka o podanym ID nie istnieje lub nie należy do użytkownika
  - 500 Internal Server Error – błąd po stronie serwera

## 5. Przepływ danych
1. **Autoryzacja:**  
   - Weryfikacja tokena JWT z nagłówka `Authorization` celem uzyskania tożsamości użytkownika.
   
2. **Walidacja wejścia:**  
   - Sprawdzenie poprawności i kompletności danych w ciele żądania przy użyciu atrybutów walidujących w `UpdateCardCommand` (długości pól, wymagane pola).
   
3. **Sprawdzenie własności zasobu:**  
   - Zapytanie do bazy danych o fiszkę o podanym `id`.  
   - Weryfikacja, czy fiszka należy do użytkownika (zgodnie z polityką RLS).

4. **Aktualizacja:**  
   - Wywołanie logiki biznesowej w odpowiedniej warstwie serwisowej (np. `CardService`) w celu przeprowadzenia aktualizacji rekordu w bazie danych.
   - Aktualizacja pól `front` i `back` oraz ustawienie znacznika `updated_at`.

5. **Odpowiedź:**  
   - Zwrócenie zaktualizowanego obiektu `CardDto` jako odpowiedź w formacie JSON wraz ze statusem 200 OK.

## 6. Względy bezpieczeństwa
- **Autoryzacja:**  
  - Wymagany token JWT w nagłówku `Authorization`.  
  - Upewnienie się, że użytkownik ma dostęp tylko do swoich fiszek.
  
- **Walidacja danych:**  
  - Użycie wbudowanych walidatorów (atrybuty w `UpdateCardCommand`) w celu ochrony przed nieprawidłowymi danymi.
  
- **RLS (Row-Level Security):**  
  - Skonfigurowane na poziomie bazy danych, aby tylko właściciel mógł modyfikować swoje fiszki.

## 7. Obsługa błędów
- **400 Bad Request:**  
  - Gdy dane przekazane w żądaniu nie spełniają kryteriów walidacyjnych (np. puste lub nieprawidłowe długości pól).
  
- **401 Unauthorized:**  
  - Gdy token JWT jest nieobecny, nieważny lub niepoprawny.
  
- **404 Not Found:**  
  - Gdy fiszka o podanym `id` nie istnieje lub użytkownik nie ma do niej dostępu.
  
- **500 Internal Server Error:**  
  - Niespodziewane błędy podczas przetwarzania żądania lub przy operacjach na bazie danych.  
  - Logowanie błędów do systemu (ewentualnie do dedykowanej tabeli błędów, jeśli taka funkcjonalność jest dostępna).

## 8. Rozważania dotyczące wydajności
- Operacja aktualizacji pojedynczego rekordu jest mało zasobożerna.  
- Optymalizacja zapytań (indeksy na polach `id` i `user_id`) zapewni szybką wyszukiwarkę rekordu.  
- Ewentualne logowanie błędów nie powinno wpływać na wydajność; warto rozważyć asynchroniczne logowanie w przypadku dużej liczby operacji.

## 9. Etapy wdrożenia
1. **Setup autoryzacji:**  
   - Upewnić się, że middleware rozpoznaje i weryfikuje tokeny JWT.
   
2. **Implementacja walidacji:**  
   - Dodanie walidacji ciała żądania przy użyciu modelu `UpdateCardCommand`.
   
3. **Przygotowanie logiki serwisowej:**  
   - Utworzyć lub rozszerzyć istniejący serwis (np. `CardService`) do obsługi aktualizacji fiszki.
   - Uwzględnić sprawdzenie własności zasobu i aktualizację pola `updated_at`.
   
4. **Integracja z bazą danych:**  
   - Wykonanie zapytań aktualizujących rekord w tabeli `cards` zgodnie z polityką RLS.
   - Upewnienie się, że zapytania wykonują się przy użyciu transakcji, w razie potrzeby.
   
5. **Obsługa błędów:**  
   - Dodanie mechanizmów łapania błędów i zwracania odpowiednich kodów HTTP oraz logowania błędów.
   
6. **Testy:**  
   - Utworzenie testów jednostkowych i integracyjnych, które sprawdzą:
     - Pomyślną aktualizację fiszki,
     - Obsługę nieprawidłowych danych wejściowych,
     - Brak dostępu przy niewłaściwym tokenie,
     - Scenariusz gdy fiszka nie istnieje.
   
7. **Dokumentacja:**  
   - Aktualizacja dokumentacji API, uwzględniając szczegóły nowego endpointu.
   
8. **Deploy i monitorowanie:**  
   - Wdrożenie endpointu na środowisko testowe, przeprowadzenie testów bezpieczeństwa oraz monitorowanie logów.