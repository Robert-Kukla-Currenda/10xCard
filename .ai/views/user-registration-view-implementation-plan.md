# Plan implementacji widoku Rejestracja Użytkownika

## 1. Przegląd
Widok rejestracji pozwala nowym użytkownikom utworzyć konto, podając adres email, imię, nazwisko oraz hasło. Jego głównym celem jest zebranie niezbędnych informacji w intuicyjnym i bezpiecznym formularzu.

## 2. Routing widoku
Widok będzie dostępny pod ścieżką:  
`/register`

## 3. Struktura komponentów
- **RegisterPage** (główny komponent strony `/register`)
  - **RegistrationForm** (formularz rejestracyjny odpowiedzialny za przechowywanie danych użytkownika)
  - **ToastNotifications** (komponent odpowiedzialny za wyświetlanie komunikatów o błędach i sukcesach)

## 4. Szczegóły komponentów

### RegisterPage
- **Opis**: Główny kontener, który ustawia layout, zawiera nagłówek i komponent formularza oraz wyświetla powiadomienia (toast).
- **Główne elementy**:
  - Tytuł strony (np. "Zarejestruj się")
  - Komponent `RegistrationForm`
  - Komponent `ToastNotifications`
- **Obsługiwane interakcje**:
  - Brak interakcji bezpośrednich poza wyświetleniem formularza
- **Obsługiwana walidacja**: Przekazywana dalej do `RegistrationForm`
- **Typy**: Nie definiuje własnych typów; wykorzystuje typy z formularza
- **Propsy**: Brak dodatkowych propsów przekazywanych z rodzica

### RegistrationForm
- **Opis komponentu**: Renderuje formularz rejestracji na podstawie pól: email, imię, nazwisko oraz hasło. Komunikuje się z API w celu utworzenia nowego konta.
- **Główne elementy**:
  - Pola tekstowe: `Email`, `FirstName`, `LastName`, `Password`
  - Przycisk `Zarejestruj się`
- **Obsługiwane interakcje**:
  - Wpisywanie tekstu do pól, dynamiczna walidacja inline
  - Kliknięcie w przycisk `Zarejestruj się` – wysyłka żądania rejestracji
- **Warunki walidacji**:
  - Email: wymagany, format email, maks. 255 znaków
  - Imię: wymagane, maks. 100 znaków
  - Nazwisko: wymagane, maks. 100 znaków
  - Hasło: wymagane, 6-100 znaków, co najmniej jedna mała i wielka litera oraz cyfra
- **Typy**:
  - `RegisterUserCommand`: opis struktury danych wysyłanych do API
- **Propsy**: Brak zewnętrznych propsów; komponent sam zarządza swoimi danymi

### ToastNotifications
- **Opis komponentu**: Wyświetla krótkotrwałe powiadomienia o wyniku rejestracji lub błędach.
- **Główne elementy**: Lista "toastów" do prezentacji komunikatów sukcesu lub błędów
- **Obsługiwane interakcje**: Brak interakcji od użytkownika, komponent reaguje na zmiany w stanie aplikacji
- **Warunki walidacji**: Brak
- **Typy**: Brak nowych
- **Propsy**: Oczekuje listy wiadomości do wyświetlenia i typ działania (sukces/błąd)

## 5. Typy
- **RegisterUserCommand**  
  - `email: string`  
  - `firstName: string`  
  - `lastName: string`  
  - `password: string`  

Formularz korzysta z powyższych pól przy wywołaniu API.

## 6. Zarządzanie stanem
- Formularz przechowuje wartości pól w stanie komponentu (np. przez użycie hooków React).  
- Walidacja inline: błędy przechowywane tymczasowo w stanie, aktualizowane po każdej zmianie w polu.

## 7. Integracja API
- **Metoda**: POST  
- **Endpoint**: `/users/register`  
- **Żądanie**: obiekt `RegisterUserCommand` w formacie JSON  
- **Odpowiedź**: jeżeli 201 (Created), zawiera nowo utworzonego użytkownika.  
- **Obsługa błędów**:  
  - 400 (Bad Request) – błędy walidacji  
  - 409 (Conflict) – istniejący adres email

## 8. Interakcje użytkownika
1. Użytkownik wypełnia formularz: wypełnienie pól i weryfikacja inline (np. nieprawidłowy adres email).  
2. Kliknięcie w `Zarejestruj się`: dane są wysyłane do API, a w przypadku sukcesu użytkownik otrzymuje powiadomienie o pomyślnym utworzeniu konta.  
3. Błędy walidacji lub konflikt adresu email wyświetlane w postaci toastów lub inline.

## 9. Warunki i walidacja
- Sprawdzenie poprawności emaila i formatu hasła przed wysłaniem żądania.  
- W odpowiedzi od API weryfikacja kodu statusu (201, 400, 409).

## 10. Obsługa błędów
- **Kod 400** – wyświetlenie komunikatu o nieprawidłowych danych w formularzu.  
- **Kod 409** – informacja o zajętym adresie email.  
- **Kod 500** – komunikat o błędzie serwera (nieoczekiwany błąd).

## 11. Kroki implementacji (z uwzględnieniem .NET i Blazor)
1. Utworzyć nowy komponent Blazor (np. `Register.razor`) zawierający formularz rejestracji (pola: `Email`, `FirstName`, `LastName`, `Password`). 
2. Zaimportować i zainicjować modele danych z .NET (np. `RegisterUserCommand`), aby zapewnić spójność pomiędzy warstwą frontendu (Blazor) i backendem (API .NET). 
3. Zaimplementować walidację wbudowaną (DataAnnotations) oraz dodatkową logikę w kodzie Blazor (np. w metodzie `OnValidSubmit`) dla poprawności pól. 
4. Po wciśnięciu przycisku „Zarejestruj się” wysłać żądanie POST `/users/register` do kontrolera .NET z wykorzystaniem usługi HTTP w Blazor (np. `HttpClient`).  
5. Obsłużyć odpowiedź API – w przypadku sukcesu wyświetlić komunikat o pomyślnej rejestracji; w przypadku błędu pokazać odpowiedni komunikat (np. `409 Conflict` dla istniejącego emaila). 
6. Zadbać o silne typowanie (np. DTO `UserDto`) oraz mapowanie danych w celu wyświetlenia odpowiednich komunikatów i ewentualnej nawigacji do ekranu logowania. 
7. Otestować formularz manualnie i automatycznie (np. testy jednostkowe i integracyjne), sprawdzając poprawność przepływu rejestracji i obsługi błędów.