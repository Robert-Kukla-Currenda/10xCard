# Plan wdrożenia endpointu API: Rejestracja użytkownika

## 1. Przegląd punktu końcowego
Endpoint ten służy do tworzenia nowego konta użytkownika. Jego zadaniem jest przyjęcie danych rejestracyjnych, walidacja wejścia, bezpieczne hashowanie hasła, sprawdzenie unikalności adresu e-mail oraz zapisanie nowego rekordu w bazie danych. Po poprawnym utworzeniu użytkownika, zwracane są dane użytkownika (bez pola hasła). Endpoint jest kluczowym elementem procesu uwierzytelniania i musi być zabezpieczony przed wprowadzeniem nieprawidłowych danych oraz próbami rejestracji z już istniejącym adresem e-mail.

## 2. Szczegóły żądania
- **Metoda HTTP:** POST  
- **Struktura URL:** `/users/register`  
- **Parametry:**
  - **Wymagane (w treści żądania w formacie JSON):**
    - `email` (string): Adres e-mail użytkownika; musi być poprawnie sformatowany i unikalny.
    - `first_name` (string): Imię użytkownika.
    - `last_name` (string): Nazwisko użytkownika.
    - `password` (string): Hasło użytkownika w postaci tekstu jawnego (hasło zostanie zahashowane po stronie serwera).
  - **Opcjonalne:** Brak

- **Przykład treści żądania:**
    ```json
    {
      "email": "user@example.com",
      "first_name": "John",
      "last_name": "Doe",
      "password": "PlainTextPassword"
    }
    ```

## 3. Wykorzystywane typy
- **Command Model:** `RegisterUserCommand`  
  (_Definiowany w pliku Commands.cs_)  
  - Zawiera pola: `Email`, `FirstName`, `LastName`, `Password`.

- **DTO:** `UserDto`  
  (_Definiowany w pliku DTOs.cs_)  
  - Zawiera pola: `Id`, `Email`, `FirstName`, `LastName`, `CreatedAt`.

## 4. Szczegóły odpowiedzi
- **W przypadku powodzenia:**
  - **Kod statusu:** 201 Created  
  - **Przykładowa treść odpowiedzi:**
    ```json
    {
      "id": 1,
      "email": "user@example.com",
      "first_name": "John",
      "last_name": "Doe",
      "created_at": "2025-04-19T12:00:00Z"
    }
    ```

- **W przypadku błędów:**
  - **400 Bad Request:** Gdy dane wejściowe nie przejdą walidacji (np. niepoprawny format adresu e-mail, brak wymaganych pól).
  - **409 Conflict:** Gdy użytkownik z podanym adresem e-mail już istnieje.

## 5. Przepływ danych
1. **Odbiór żądania i walidacja:**  
   Żądanie jest deserializowane do modelu `RegisterUserCommand`. Wykorzystujemy mechanizmy walidacji (np. atrybuty data annotations lub użycie Zod w przypadku Astro backend) do weryfikacji poprawności pól.
2. **Hashowanie hasła:**  
   Przed zapisaniem do bazy danych, hasło jest bezpiecznie hashowane (np. przy użyciu algorytmu BCrypt).
3. **Sprawdzenie unikalności e-mail:**  
   Serwis użytkowników wykonuje zapytanie do bazy PostgreSQL w celu sprawdzenia, czy dany adres e-mail już istnieje (w kolumnie z indeksem unikalnym).
4. **Zapis rekordu w bazie danych:**  
   Przy użyciu Entity Framework lub Supabase (zgodnie z wybranym back-endem), tworzony jest nowy rekord użytkownika, w tym hash hasła oraz znacznik czasu utworzenia.
5. **Mapowanie danych:**  
   Po zapisaniu, rekord jest mapowany przy użyciu `UserDto` i zwracany do klienta jako odpowiedź.

## 6. Względy bezpieczeństwa
- **Bezpieczeństwo hasła:**  
  Hasło jest hashowane przy użyciu silnego algorytmu (np. BCrypt), a hasło w postaci jawnej nigdy nie jest przechowywane ani logowane.
- **Walidacja wejścia:**  
  Dane wejściowe są skrupulatnie walidowane, aby zapobiec atakom wstrzykiwania oraz zapewnić zgodność z ograniczeniami bazy danych (m.in. format e-mail, długość pól).
- **Ograniczenia rate limiting:**  
  Endpoint powinien być zabezpieczony mechanizmem rate limiting, aby zapobiec próbą brute-force oraz nadużyciom.
- **Kontrola błędów:**  
  W przypadku wystąpienia błędów, serwer zwraca standardowe kody HTTP, a szczegóły nie są ujawniane klientowi, aby nie dostarczać potencjalnych wektorów ataku.

## 7. Obsługa błędów
- **400 Bad Request:**  
  Jeśli walidacja danych wejściowych nie powiedzie się, zwracamy odpowiednią wiadomość o błędzie np. „Niepoprawny format e-mail” lub „Brak wymaganych pól”.
- **409 Conflict:**  
  Jeśli użytkownik z tym adresem e-mail już istnieje, zwracamy błąd konfliktu.
- **500 Internal Server Error:**  
  Dla nieoczekiwanych wyjątków wewnętrznych serwera, z odpowiednim logowaniem błędów (opcjonalnie zanotowanie błędu w tabeli `error_logs`, jeśli jest stosowane w innych endpointach).

## 8. Rozważania dotyczące wydajności
- **Indeksowanie:**  
  Kolumna `email` posiada indeks unikalny, co przyspiesza sprawdzanie istnienia użytkownika.
- **Asynchroniczność:**  
  Wdrożenie asynchronicznych metod dostępu do bazy danych w celu zwiększenia przepustowości i skalowalności.
- **Optymalizacja:**  
  Monitorowanie i profilowanie zapytań do bazy danych, aby upewnić się, że operacje rejestracji nie powodują wąskich gardeł.

## 9. Etapy wdrożenia
1. **Konfiguracja endpointu:**
   - Utworzenie nowej ścieżki `/users/register` w kontrolerze API.
   - Ustawienie metody na POST oraz powiązanie danych żądania z modelem `RegisterUserCommand`.
2. **Implementacja warstwy serwisowej:**
   - Utworzenie serwisu (np. `UserService`) zawierającego logikę rejestracji użytkownika.
   - Wdrożenie walidacji, hash'owania hasła i sprawdzania unikalności e-mail.
3. **Integracja z bazą danych:**
   - Użycie Entity Framework/Supabase do zapisywania nowego rekordu użytkownika.
   - Zapewnienie obsługi transakcji oraz właściwego logowania błędów.
4. **Mapowanie na DTO:**
   - Po utworzeniu użytkownika, mapowanie rekordu na `UserDto`.
   - Wykorzystanie narzędzi do mapowania (AutoMapper) lub ręczne mapowanie.
5. **Obsługa błędów:**
   - Implementacja middleware do obsługi wyjątków i zwracania standardowych kodów błędów.
   - Dodanie logowania błędów (opcjonalnie integracja z zewnętrznymi systemami logowania).
6. **Testy jednostkowe i integracyjne:**
   - Przygotowanie testów sprawdzających poprawność walidacji, hash'owania i obsługi konfliktów.
   - Testy endpointu pod kątem reakcji na poprawne i nieprawidłowe dane wejściowe.
7. **Przegląd bezpieczeństwa i optymalizacja:**
   - Weryfikacja implementacji przez audyt bezpieczeństwa.
   - Przeprowadzenie testów obciążeniowych, aby upewnić się o wysokiej wydajności endpointu.
8. **Dokumentacja i Code Review:**
   - Szczegółowa dokumentacja endpointu.
   - Przegląd kodu przez zespół programistyczny w celu zatwierdzenia zgodności z najlepszymi praktykami.