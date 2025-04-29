# API Endpoint Implementation Plan: Get Single Card

## 1. Przegląd punktu końcowego
Endpoint GET /cards/{id} umożliwia pobranie pojedynczej fiszki na podstawie jej ID. Endpoint zabezpiecza autoryzacja JWT, a użytkownik może uzyskać dostęp jedynie do swoich fiszek.

## 2. Szczegóły żądania
- **Metoda HTTP:** GET
- **Struktura URL:** /cards/{id}
- **Parametry:**
  - **Wymagane:** 
    - ID fiszki (parametr ścieżki)
    - Header "Authorization" z tokenem JWT
  - **Opcjonalne:** Brak
- **Request Body:** Brak

## 3. Wykorzystywane typy
- **DTO:** 
  - CardDto (definiuje strukturę odpowiedzi z danymi fiszki)

## 4. Szczegóły odpowiedzi
- **Kod 200 OK:** Zwraca obiekt typu CardDto, przykładowa odpowiedź:
  ```json
  {
    "id": 102,
    "user_id": 1,
    "original_content": "Long text with 1000 to 10000 characters...",
    "front": "Generated summary question...",
    "back": "Generated detailed answer...",
    "generated_by": "AI",
    "created_at": "2025-04-19T12:10:00Z"
  }
  ```
- **Kod 401 Unauthorized:** W przypadku braku lub niepoprawnego tokenu.
- **Kod 404 Not Found:** Gdy fiszka o zadanym ID nie istnieje.
- **Kod 500 Internal Server Error:** W przypadku nieoczekiwanych błędów serwera.

## 5. Przepływ danych
1. Klient wysyła żądanie GET /cards/{id} z tokenem autoryzacyjnym w headerze.
2. Warstwa autoryzacji weryfikuje token JWT.
3. Kontroler/endpoint wywołuje metodę w serwisie (np. CardService) z ID fiszki.
4. Serwis pobiera dane fiszki z bazy danych, stosując filtry dostępu (RLS) określające, że użytkownik może uzyskać dostęp tylko do swoich fiszek.
5. W przypadku znalezienia, dane są mapowane do obiektu CardDto i zwracane do klienta.
6. W razie niepowodzenia (brak autoryzacji lub nieznalezienie zasobu) odpowiednie kody błędów są zwracane.

## 6. Względy bezpieczeństwa
- Weryfikacja tokena JWT w headerze "Authorization".
- Implementacja RLS zapewniająca, że użytkownik może odczytać tylko swoje fiszki.
- Walidacja parametru ścieżki (id musi być liczbą).
- Ochrona przed wyciekiem wewnętrznych szczegółów błędów, aby nie ujawniać informacji o backendzie.

## 7. Obsługa błędów
- **401 Unauthorized:** Gdy brakuje lub jest niepoprawny token, endpoint zwraca komunikat o braku dostępu.
- **404 Not Found:** Gdy fiszka o zadanym ID nie istnieje lub użytkownik nie ma do niej dostępu.
- **500 Internal Server Error:** Logowanie błędów serwera (opcjonalnie rejestracja błędów w tabeli error logs), przy wystąpieniu nieoczekiwanych wyjątków.
- Każdy błąd powinien być logowany, a logi powinny być używane do analizy problemów (opcjonalne rozszerzenie poprzez zapis w ErrorLogDto).

## 8. Rozważania dotyczące wydajności
- Korzystanie z ORM i przygotowanych zapytań zapewnia bezpieczny oraz wydajny dostęp do bazy danych.
- Mechanizmy cache’ujące (o ile dostępne) mogą być wykorzystane przy częstych zapytaniach.
- Optymalizacja zapytań SQL, aby unikać przeciążenia bazy danych przy dużej liczbie równoczesnych żądań.

## 9. Etapy wdrożenia
1. **Przygotowanie endpointu:**
   - Zdefiniowanie routingu w projekcie, aby skierować zapytania GET /cards/{id} do odpowiedniego kontrolera.

2. **Autoryzacja:**
   - Implementacja middleware lub wykorzystanie istniejącej logiki autoryzacji w celu weryfikacji tokenu JWT.

3. **Walidacja wejściowa:**
   - Walidacja parametru "id" (sprawdzenie, czy jest liczbą).
   - Zapewnienie, że header "Authorization" zawiera poprawny token.

4. **Integracja z serwisem:**
   - Utworzenie lub rozszerzenie serwisu (np. CardService) odpowiedzialnego za pobieranie fiszki z bazy danych.
   - Zaimplementowanie logiki sprawdzającej RLS – zwracanie fiszki tylko, jeśli użytkownik jest jej właścicielem.

5. **Mapowanie danych:**
   - Mapowanie pobranych danych z bazy do obiektu CardDto zgodnie z definicją DTO.

6. **Obsługa błędów i logowanie:**
   - Obsługa wyjątków i zwracanie odpowiednich kodów błędów (401, 404, 500).
   - Opcjonalne logowanie błędów do tabeli error logs w przypadku problemów.

7. **Testowanie:**
   - Implementacja testów jednostkowych oraz testów integracyjnych, aby upewnić się, że endpoint zwraca prawidłowe wyniki w scenariuszach sukcesu oraz błędów.
   - Uwzględnienie testów dla przypadków braku tokena, niepoprawnego tokena, nieistniejącej fiszki oraz dostępu do cudzych zasobów.

8. **Wdrożenie:**
   - Przeprowadzenie code review.
   - Wdrożenie do środowiska staging.
   - Monitorowanie wydajności i logów przed wdrożeniem do produkcji.
