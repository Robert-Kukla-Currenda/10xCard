# Plan wdrożenia usługi OpenRouter

## 1. Opis usługi  
Usługa OpenRouter ma za zadanie integrację z interfejsem API Openrouter.ai w celu uzupełnienia komunikacji czatów opartych na LLM. Główne zadania usługi obejmują:
1. Przekazywanie komunikatów systemowych oraz użytkownika do API Openrouter.ai.
2. Przetwarzanie odpowiedzi od API, w tym walidację zgodności z ustalonym schematem JSON.
3. Konfigurację parametrów wywołania modelu (nazwa modelu, temperatura, maksymalna liczba tokenów) w celu osiągnięcia optymalnej wydajności oraz kontroli kosztów.
4. Zabezpieczenie i obsługę błędów w całym przepływie komunikacyjnym.

## 2. Opis konstruktora  
Konstruktor usługi powinien:
- Inicjalizować podstawowe parametry, takie jak domyślna nazwa modelu (np. `"gpt-4o-mini"`) oraz ustawienia modelu (np. `temperature`, `max_tokens`).
- Przyjmować opcjonalne argumenty konfiguracyjne, które umożliwią modyfikację parametrów wywołań w czasie działania.
- Konfigurować elementy logowania błędów, co jest kluczowe w środowisku .NET 8, wykorzystując wbudowane mechanizmy logowania lub zewnętrzne biblioteki.
- Ustanawiać bezpieczny dostęp do kluczy API, korzystając z mechanizmu zmiennych środowiskowych (np. przy użyciu Secret Manager lub konfiguracji aplikacji).

## 3. Publiczne metody i pola  
**Metody:**  
1. `Task<Response> SendMessageAsync(string message, MessageRole role)`  
   - Metoda umożliwia wysłanie komunikatu (systemowego lub użytkownika) do Openrouter.ai. W zależności od roli wiadomości, metoda przygotowuje odpowiedni payload i wykonuje asynchroniczne wywołanie API.
2. `ResponseFormat GetResponseFormat()`  
   - Zwraca poprawnie skonfigurowany format odpowiedzi zgodny z wymogiem:  
     `{ type: 'json_schema', json_schema: { name: [schema-name], strict: true, schema: [schema-obj] } }`.

**Pola:**  
1. `string DefaultModelName`  
   - Przechowuje domyślną nazwę modelu (np. `"gpt-4o-mini"`).
2. `ModelParameters ModelParameters`  
   - Zawiera ustawienia wywołań, takie jak `temperature` czy `max_tokens`.
3. `string ApiEndpoint`  
   - URL endpointu Openrouter.ai, pobierany z konfiguracji aplikacji lub zmiennych środowiskowych.

## 4. Prywatne metody i pola  
**Metody:**  
1. `bool ValidateResponse(object response)`  
   - Waliduje odpowiedź otrzymaną z API według zdefiniowanego schematu JSON, używając narzędzi walidacji dostępnych w .NET lub zewnętrznych bibliotek (np. JSON Schema Validator).
2. `void LogError(Exception ex)`  
   - Centralna metoda logowania błędów, wykorzystująca wbudowany system logowania .NET (ILogger) lub inne dedykowane narzędzia.
3. `Payload PreparePayload(string message, MessageRole role)`  
   - Metoda przygotowująca dane (payload) do wysłania do API. Uwzględnia przy tym:
   - Komunikat systemowy
   - Komunikat użytkownika
   - Response_format zgodny z ustalonym schematem
   - Nazwę modelu i parametry modelu

**Pola:**  
1. `Payload PayloadTemplate`  
   - Szablon pakietu danych, który jest modyfikowany przy każdym wywołaniu API.

## 5. Obsługa błędów  
Przy wdrażaniu usługi należy uwzględnić następujące scenariusze błędów:
1. **Błąd połączenia z API**  
   - *Problem:* Timeout lub niedostępność endpointu.
   - *Rozwiązanie:* Wprowadzenie retry logic, alternatywnego endpointu lub informowanie użytkownika o problemach z siecią.
2. **Błąd walidacji odpowiedzi**  
   - *Problem:* Odpowiedź nie spełnia wymaganego schematu JSON.
   - *Rozwiązanie:* Zaimplementowanie walidatora, który generuje szczegółowy komunikat błędu oraz loguje niezgodności.
3. **Błąd konfiguracji modelu**  
   - *Problem:* Brak lub niepoprawne dane konfiguracyjne.
   - *Rozwiązanie:* Ustanowienie domyślnych wartości, walidacja konfiguracji i natychmiastowe zgłaszanie błędu.
4. **Błąd autoryzacyjny**  
   - *Problem:* Błędne lub wygasłe klucze API.
   - *Rozwiązanie:* Weryfikacja kluczy API przy starcie usługi oraz stosowanie mechanizmów zabezpieczających dostępy.

## 6. Kwestie bezpieczeństwa  
- **Walidacja danych:**  
  Wszystkie dane wejściowe i wyjściowe muszą podlegać walidacji przy użyciu mechanizmów dostępnych w .NET 8 (np. z wykorzystaniem biblioteki FluentValidation).
- **Bezpieczne przechowywanie kluczy API:**  
  Klucze oraz wrażliwe dane powinny być przechowywane jako zmienne środowiskowe, przy użyciu np. Secret Managera.
- **Ochrona przed nadużyciami:**  
  Mechanizmy rate limiting oraz retry logic chronią przed nadmiernymi wywołaniami API.
- **Logowanie bez ujawniania danych wrażliwych:**  
  Implementacja bezpiecznego logowania, która nie ujawnia szczegółów błędów w środowisku produkcyjnym.

## 7. Plan wdrożenia krok po kroku  
1. **Inicjalizacja projektu i konfiguracja środowiska:**  
   - Upewnić się, że projekt korzysta ze stosu technologicznego:  
     - Frontend: Blazor (z MudBlazor oraz TailwindCSS)  
     - Backend: .NET 8, Entity Framework, PostgreSQL  
     - AI: Integracja z Openrouter.ai
   - Skonfigurować zmienne środowiskowe zawierające klucze API oraz endpoint Openrouter.ai.

2. **Implementacja konstruktora i konfiguracji usługi:**  
   - Utworzyć klasę `OpenRouterService` w katalogu `./src/lib/services`.
   - Zainicjować domyślne wartości (DefaultModelName, ModelParameters) oraz ustawić ApiEndpoint.

3. **Implementacja kluczowych komponentów:**  
   - **Komponent 1: Przygotowanie Payload**  
     - Zaimplementować metodę `PreparePayload`, która formuje dane do wysłania do API, zawierając:
       1. **Komunikat systemowy:**  
          Przykład:  
          `"system": "Instrukcje systemowe dla LLM, dotyczące trybu pracy usługi."`
       2. **Komunikat użytkownika:**  
          Przykład:  
          `"user": "Treść wiadomości użytkownika do przetworzenia."`
       3. **Response Format:**  
          Ustawić zgodnie ze wzorem:  
          ```json
          { 
            "type": "json_schema", 
            "json_schema": { 
              "name": "chatCompletionResponse", 
              "strict": true, 
              "schema": { "result": "string" } 
            } 
          }
          ```
       4. **Nazwa modelu:**  
          Przykład: `"gpt-4o-mini"`.
       5. **Parametry modelu:**  
          Przykład: `{ "temperature": 0.7, "max_tokens": 256 }`.
   - **Komponent 2: Wysyłanie żądania**  
     - Zaimplementować metodę `SendMessageAsync`, która wykonuje asynchroniczne wywołanie HTTP do API Openrouter.ai, korzystając z przygotowanego payload.
     - Obsłużyć odpowiedź przy pomocy metody `ValidateResponse` oraz odpowiednio logować i obsługiwać błędy.

4. **Walidacja odpowiedzi i obsługa błędów:**  
   - Zaimplementować metodę `ValidateResponse` do sprawdzania zgodności otrzymanej odpowiedzi z ustalonym schematem.
   - Dodać retry logic w przypadku błędów sieciowych lub autoryzacyjnych.
   - Wdrożyć centralne logowanie błędów korzystając z wbudowanego systemu logowania .NET.
   - Dodać mechanizm zapisywania błędów związanych z generowaniem fiszek do odpowiedniej tabeli w bazie danych (error_logs).
     - Mechanizm realizowany przy użyciu ORM (np. Entity Framework).
     - Szczegóły zapisu obejmują: identyfikator fiszki (card_id), komunikat błędu, a także znacznik czasu.
     - Upewnić się, że logowane dane nie ujawniają wrażliwych informacji, zgodnie z zasadami z pliku copilot-instructions.md.

5. **Wdrożenie i monitorowanie:**  
   - Wdrożyć implementację w środowisku testowym, konfigurując logi oraz monitorowanie (np. przy użyciu Application Insights).
   - Utworzyć pipeline CI/CD przy użyciu GitHub Actions oraz opakować aplikację w kontener Docker do hostowania na DigitalOcean.
   - Monitorować wydajność, zużycie API oraz ewentualne błędy w czasie rzeczywistym.

6. **Dokumentacja i przekazanie:**  
   - Sporządzić dokumentację opisującą wszystkie publiczne metody, przykłady zastosowania i konfigurację usługi.
   - Przeprowadzić szkolenie oraz omówienie implementacji z developerami odpowiedzialnymi za utrzymanie systemu.
   - Po dokładnym przetestowaniu wdrożyć usługę w środowisku produkcyjnym.
