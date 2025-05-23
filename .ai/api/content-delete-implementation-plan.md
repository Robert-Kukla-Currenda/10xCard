# API Endpoint Implementation Plan: Delete Original Content

## 1. Przegląd punktu końcowego
Endpoint umożliwia usunięcie treści oryginalnej oraz wszystkich powiązanych z nią fiszek. Jest to operacja krytyczna, wymagająca odpowiedniej weryfikacji użytkownika przy wykorzystaniu tokena JWT oraz zabezpieczeń, aby jedynie właściciel mógł dokonać usunięcia.

## 2. Szczegóły żądania
- **Metoda HTTP:** DELETE  
- **Struktura URL:** `/original-contents/{id}`  
- **Parametry:**  
  - **Wymagane:**  
    - `id` (parametr ścieżki, liczba całkowita)  
    - Nagłówek `Authorization: Bearer <JWT_TOKEN_HERE>`
  - **Opcjonalne:** Brak
- **Request Body:** Brak

## 3. Wykorzystywane typy
- **DTOs:**  
  - `OriginalContentDto` – reprezentuje rekord treści oryginalnej.
  - `CardDto` – reprezentuje powiązane fiszki (w relacji jeden-do-wielu).
- **Command Model:**  
  - Brak dedykowanego commandu, gdyż operacja została określona wyłącznie przez identyfikator w URL i nagłówek autoryzacji.

## 4. Szczegóły odpowiedzi
- **Sukces (200 OK):**  
  ```json
  {
    "message": "Original content and associated cards deleted successfully."
  }
  ```
- **Błędy:**  
  - **401 Unauthorized:** Jeśli token JWT jest niepoprawny lub brak jego obecności.
  - **404 Not Found:** Jeśli treść oryginalna o podanym id nie istnieje.
  - **500 Internal Server Error:** W przypadku nieoczekiwanych błędów serwera.

## 5. Przepływ danych
1. Klient wysyła żądanie DELETE do `/original-contents/{id}` wraz z nagłówkiem Authorization.
2. Serwer weryfikuje token JWT i autoryzuje użytkownika.
3. Pobierany jest rekord treści oryginalnej o podanym id (oraz sprawdzana przynależność do użytkownika).
4. Rozpoczynana jest transakcja, podczas której usuwana jest treść oryginalna oraz wszystkie powiązane fiszki.
5. Transakcja zostaje zatwierdzona, a serwer zwraca komunikat sukcesu.

## 6. Względy bezpieczeństwa
- **Autoryzacja:**  
  - Weryfikacja tokena JWT w nagłówku Authorization.
  - Sprawdzenie, czy użytkownik posiada uprawnienia do usunięcia danej treści (np. porównanie user_id w rekordzie treści z id z tokena).
- **Walidacja:**  
  - Walidacja formatu i typu parametru `id`.
  - Użycie mechanizmów ORM (np. Entity Framework) w celu ochrony przed SQL Injection.
- **Logowanie:**  
  - Rejestrowanie prób nieautoryzowanego dostępu lub błędów związanych z operacją usunięcia.

## 7. Obsługa błędów
- **401 Unauthorized:**  
  - Zwrócić błąd, gdy token JWT jest niepoprawny lub zabrak go.
- **404 Not Found:**  
  - Zwrócić błąd, gdy rekord treści oryginalnej nie został znaleziony.
- **500 Internal Server Error:**  
  - Zajmować się nieoczekiwanymi błędami przy operacji i logować je dla dalszej analizy.
- Dodatkowo, opcjonalnie można zapisywać błędy w systemie logowania przy użyciu ErrorLogDto, jeżeli operacja nie powiodła się z powodu wewnętrznych problemów.

## 8. Rozważania dotyczące wydajności
- Użycie mechanizmu transakcji przy usuwaniu, aby uniknąć niekompletnych operacji – gwarantuje spójność danych.
- Wykorzystanie mechanizmów ORM (np. Entity Framework) zapewniających optymalizację zapytań.
- Dbanie o indeksy na kluczach obcych, aby zapewnić szybkie operacje wyszukiwania.
- Monitoring operacji usunięcia, aby zidentyfikować ewentualne wąskie gardła w bazie danych.

## 9. Etapy wdrożenia
1. **Przygotowanie środowiska:**
   - Upewnić się, że konfiguracja tokena JWT i autoryzacja działają poprawnie.
   - Zweryfikować, że baza danych posiada odpowiednie klucze i indeksy.

2. **Implementacja logiki serwisu:**
   - Utworzyć lub zaktualizować serwis np. w `src/lib/services/OriginalContentService.ts` (w przypadku .NET odpowiedni serwis w warstwie backendu).
   - Zaimplementować metodę usuwającą treść oryginalną i powiązane fiszki w ramach transakcji.

3. **Wdrożenie Endpointu REST API:**
   - Utworzyć endpoint DELETE `/original-contents/{id}` w wybranej warstwie API.
   - Dodać middleware do autoryzacji JWT i walidacji parametrów.

4. **Testowanie:**
   - Utworzyć testy jednostkowe i integracyjne sprawdzające poprawność autoryzacji, walidacji oraz logiki usuwania.
   - Przetestować scenariusze: sukces, brak autoryzacji, rekord nieznaleziony, błąd serwera.

5. **Logowanie i monitoring:**
   - Upewnić się, że błędy są odpowiednio logowane.
   - Monitorować wydajność operacji usuwania.

6. **Review i Code Cleanup:**
   - Recenzja wdrożenia przez zespół.
   - Dostosowanie kodu do wytycznych czystego kodu.
   - Dodanie dokumentacji endpointu w ramach systemu dokumentacji API.

7. **Deployment:**
   - Wdrożenie zmian do środowiska testowego.
   - Po pomyślnych testach wdrożenie na produkcję.
