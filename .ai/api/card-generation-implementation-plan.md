# Plan wdrożenia endpointu REST API: Generate Card using AI

## 1. Przegląd punktu końcowego
Endpoint `/cards/generate` umożliwia autoryzowanemu użytkownikowi wygenerowanie fiszki przy użyciu zewnętrznego modułu AI. Na podstawie długiego tekstu (`original_content`) o długości od 1000 do 10000 znaków, system wywołuje usługę AI, która tworzy podsumowujące pytanie (pole `front`) oraz szczegółową odpowiedź (pole `back`).

## 2. Szczegóły żądania
- **Metoda HTTP:** POST  
- **Struktura URL:** `/cards/generate`  
- **Nagłówki:**  
  - `Authorization: Bearer JWT_TOKEN_HERE`
- **Request Body:**  
  Oczekiwany format JSON:
  ```json
  {
    "original_content": "Long text with 1000 to 10000 characters..."
  }
  ```
- **Walidacja pól:**  
  - `original_content`: Wymagane, musi mieć długość między 1000 a 10000 znaków.

## 3. Wykorzystywane typy
- **Command Model:**  
  Należy stworzyć model polecenia, np. `GenerateCardCommand` (w pliku `Commands.cs`). Model ten powinien zawierać:
  - `original_content`: string (wymagany, walidacja długości 1000–10000 znaków).
- **DTO:**  
  Model `CardDto` (w pliku `DTOs.cs`), który reprezentuje dane fiszki zwracanej w odpowiedzi. Powinien zawierać:
  - `id`
  - `user_id`
  - `original_content`
  - `front`
  - `back`
  - `generated_by` (wartość "AI")
  - `created_at`

## 4. Szczegóły odpowiedzi
- **Kod sukcesu:** 201 Created  
- **Przykładowa odpowiedź:**
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
- **Kody błędów:**  
  - 400 Bad Request – gdy `original_content` nie spełnia wymagań długości.
  - 401 Unauthorized – gdy brak ważnego tokena JWT.
  - 422 Unprocessable Entity – gdy usługa AI nie jest w stanie wygenerować fiszki (np. błąd po stronie AI).
  - 500 Internal Server Error – w przypadku niespodziewanych błędów serwera.

## 5. Przepływ danych
1. **Walidacja i autoryzacja:**  
   - Klient wysyła żądanie POST do `/cards/generate` z odpowiednim nagłówkiem autoryzacyjnym.
   - Middleware autoryzacyjne weryfikuje token JWT i ustawia identyfikator użytkownika.
   - Model `GenerateCardCommand` sprawdza, czy `original_content` jest zgodny z wymaganiami walidacyjnymi.
2. **Wywołanie usługi AI:**  
   - Po pomyślnej walidacji, kontroler wywołuje metodę w warstwie serwisowej, np. `ICardService.GenerateCardAsync`, przekazując polecenie i identyfikator użytkownika.
   - Warstwa serwisowa komunikuje się z zewnętrznym systemem AI (np. Openrouter.ai) w celu wygenerowania pól `front` oraz `back`.
3. **Odpowiedź:**  
   - Dane utworzonej fiszki są mapowane na `CardDto` i zwracane z kodem 201 Created.

## 6. Względy bezpieczeństwa
- **Autoryzacja:**  
  Endpoint dostępny tylko dla zalogowanych użytkowników – token JWT musi być sprawdzony przez system middleware.
- **Walidacja danych:**  
  - Wykorzystanie atrybutów walidacyjnych w modelu `GenerateCardCommand`.
  - Dodatkowe sprawdzenie długości tekstu przed wywołaniem usługi AI.
- **Komunikacja z zewnętrzną usługą:**  
  - Zapewnienie, że połączenie z usługą AI odbywa się przez bezpieczne protokoły (HTTPS).
  - Przechowywanie kluczy i konfiguracji API w zmiennych środowiskowych.

## 7. Obsługa błędów
- **400 Bad Request:**  
  Zwracany, gdy `original_content` jest pusty lub ma niepoprawną długość.
- **401 Unauthorized:**  
  Zwracany, gdy żądanie nie zawiera ważnego tokena JWT.
- **422 Unprocessable Entity:**  
  Zwracany, gdy usługa AI nie jest w stanie wygenerować fiszki – na przykład gdy przetwarzanie tekstu się nie powiedzie.
- **500 Internal Server Error:**  
  Zwracany przy niespodziewanych błędach, przy jednoczesnym logowaniu szczegółów błędu dla dalszej analizy.

## 8. Rozważania dotyczące wydajności
- **Asynchroniczność:**  
  Korzystanie z async/await podczas komunikacji z usługą AI.
- **Optymalizacja obciążeń:**  
  Monitorowanie wydajności usługi AI oraz serwera – skalowanie poziome w razie wzrostu liczby żądań.
- **Cache:**  
  Rozważyć buforowanie odpowiedzi w przypadku powtarzających się żądań o podobnym tekście, o ile model AI na to pozwala.

## 9. Etapy wdrożenia
1. **Implementacja modelu polecenia i DTO:**  
   - Upewnić się, że `GenerateCardCommand` (w `Commands.cs`) zawiera pole `original_content` z odpowiednią walidacją.
   - Sprawdzić, czy `CardDto` (w `DTOs.cs`) zawiera wszystkie wymagane pola, tj. `id`, `user_id`, `original_content`, `front`, `back`, `generated_by`, `created_at`.
2. **Rozwój warstwy serwisowej:**  
   - Utworzyć lub rozbudować metodę `ICardService.GenerateCardAsync`, która:
     - Waliduje dane wejściowe.
     - Komunikuje się z zewnętrzną usługą AI w celu wygenerowania pól `front` i `back`.     
3. **Implementacja endpointu w kontrolerze:**  
   - Utworzyć nowy endpoint POST `/cards/generate` w kontrolerze.
   - Pobrać identyfikator użytkownika z tokena JWT.
   - Przekazać dane do warstwy serwisowej i obsłużyć wynik, zwracając `CardDto` z kodem 201 Created.
4. **Testowanie i walidacja:**  
   - Przygotować testy jednostkowe i integracyjne dla endpointu, obejmujące poprawne przypadki, błędy walidacyjne i błędy związane z usługą AI.
5. **Dokumentacja i wdrożenie:**  
   - Zaktualizować dokumentację API (np.: Swagger/OpenAPI) w celu uwzględnienia nowego endpointu.
   - Przeprowadzić code review, wdrożyć zmiany w środowisku testowym, a następnie w produkcyjnym.
