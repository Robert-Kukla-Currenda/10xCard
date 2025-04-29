# API Endpoint Implementation Plan: Create Card

## 1. Przegląd punktu końcowego
Endpoint umożliwia zapisanie nowej fiszki (flashcard) do bazy danych. Użytkownik autoryzowany przesyła dane fiszki, które są najpierw walidowane, a następnie zapisywane. System zwraca utworzony rekord, zapewniając jednocześnie, że użytkownik może mieć dostęp tylko do swoich danych dzięki polityce RLS.

## 2. Szczegóły żądania
- **Metoda HTTP:** POST  
- **URL:** `/cards`  
- **Headers:**  
  - `Authorization: Bearer JWT_TOKEN_HERE`
- **Parametry w body:**
  - **Wymagane:**  
    - `front` (string, 1-1000 znaków)
    - `back` (string, 1-5000 znaków)
    - `generated_by` (string, wartość: "AI" lub "human")
  - **Opcjonalne:**  
    - `original_content`:  
      - Dla fiszek generowanych manualnie może być opcjonalne  
      - Dla fiszek generowanych przez AI – wymagane, długość 1000–10000 znaków
- **Struktura Request Body (przykład):**
  ```json
  {
    "front": "Question text...",
    "back": "Answer text...",
    "generated_by": "human",
    "original_content": "original text content..."
  }
  ```

## 3. Wykorzystywane typy
- **DTO (Data Transfer Object):**
  - `CardDto` (definiowany w DTOs.cs) – zawiera pola: id, user_id, front, back, generated_by, original_content, created_at, updated_at.
- **Command Model:**
  - `SaveCardCommand` (definiowany w Commands.cs) – zawiera pola: front, back, generated_by, original_content.

## 4. Szczegóły odpowiedzi
- **Struktura odpowiedzi (przykład):**
  ```json
  {
    "id": 101,
    "user_id": 1,
    "front": "Question text...",
    "back": "Answer text...",
    "generated_by": "human",
    "original_content": "Optional original text content...",
    "created_at": "2025-04-19T12:05:00Z",
    "updated_at": null
  }
  ```
- **Kody statusu:**
  - 201 Created – fiszka została utworzona poprawnie.
  - 400 Bad Request – błędy walidacji danych wejściowych.
  - 401 Unauthorized – brak właściwego tokena autoryzacyjnego.
  - 422 Unprocessable Entity – naruszenie reguł biznesowych (np. brak wymaganej zawartości przy fiszkach generowanych przez AI).

## 5. Przepływ danych
1. Klient wysyła żądanie POST do `/cards` z tokenem JWT w nagłówku oraz payloadem zawierającym dane fiszki.
2. Warstwa autoryzacji sprawdza token JWT.
3. Request trafia do kontrolera, który:
   - Wykonuje walidację danych przy użyciu atrybutów walidacyjnych (DataAnnotations) oraz dodatkowych reguł walidacji np. sprawdzających długość `original_content` dla fiszek AI.
4. Po pomyślnej walidacji przekazuje dane do warstwy serwisowej.
5. Warstwa serwisowa:
   - Weryfikuje zgodność z regułami biznesowymi.
   - Wywołuje operację zapisu fiszki w bazie danych (tabela `cards`).
   - Uwzględnia politykę RLS, aby upewnić się, że zapis dotyczy tylko danych danego użytkownika.
6. Po zapisaniu danych, serwis zwraca rekord fiszki w formacie `CardDto`.
7. Kontroler formatuje odpowiedź i zwraca kod statusu 201.

## 6. Względy bezpieczeństwa
- **Autoryzacja:** Weryfikacja tokena JWT w nagłówku `Authorization`.
- **Walidacja wejścia:** Skrupulatna walidacja długości i formatu wszystkich pól, aby zapobiec przesłaniu nieprawidłowych danych.
- **RLS:** Polityka Row-Level Security w bazie danych ogranicza dostęp do fiszek tylko dla ich właścicieli.
- **Sanityzacja danych:** Zapewnienie, że dane przesyłane do bazy są odpowiednio przefiltrowane, aby zapobiec potencjalnym atakom typu SQL Injection.

## 7. Obsługa błędów
- **400 Bad Request:** Zwracany w przypadku naruszenia ograniczeń walidacyjnych (np. za krótki tekst w polu `front` lub `back`).
- **401 Unauthorized:** Zwracany, gdy token JWT jest nieważny lub nie dostarczony.
- **422 Unprocessable Entity:** Gdy spełnione są warunki walidacyjne, ale naruszane są reguły biznesowe (np. brak wymaganej treści dla AI-generated cards).
- **500 Internal Server Error:** Zarejestrowanie nieoczekiwanych błędów w systemie logowania i zwrócenie ogólnej wiadomości błędu.

## 8. Rozważania dotyczące wydajności
- **Asynchroniczność:** Wykorzystanie asynchronicznych operacji przy komunikacji z bazą danych.
- **Konsolidacja walidacji:** Użycie wstępnej walidacji wejścia, aby szybko odrzucać niepoprawne żądania, oszczędzając zasoby systemowe.
- **Indeksy w bazie:** Upewnienie się, że tabela `cards` ma odpowiednie indeksy, aby obsłużyć zapytania związane z autoryzacją użytkowników.

## 9. Etapy wdrożenia
1. **Projekt interfejsu:**  
   - Przygotowanie kontrolera API pod kątem obsługi żądania POST `/cards`.
   - Zaimplementowanie middleware do weryfikacji tokena JWT.
2. **Walidacja:**  
   - Zastosowanie walidacji DataAnnotations na modelu `SaveCardCommand`.
   - Dodanie dodatkowych walidacji specyficznych dla reguł biznesowych (np. warunkowa walidacja `original_content`).
3. **Warstwa serwisowa:**  
   - Wyodrębnienie logiki zapisu fiszki w osobnej funkcji lub serwisie.
   - Implementacja obsługi błędów i logowania wewnątrz serwisu.
4. **Operacja zapisu:**  
   - Implementacja wywołania bazy danych z uwzględnieniem RLS.
   - Mapowanie danych z Command do DTO oraz zapis rekordu.
5. **Testy:**  
   - Przygotowanie testów jednostkowych i integracyjnych dla nowego endpointu.
   - Walidacja scenariuszy sukcesu i błędów (400, 401, 422, 500).
6. **Dokumentacja:**  
   - Aktualizacja dokumentacji API, opisanie nowego endpointu i przykładowych payloadów.
7. **Wdrożenie:**  
   - Wprowadzenie zmian do środowiska testowego.
   - Monitorowanie logów i zachowania endpointu po wdrożeniu.
