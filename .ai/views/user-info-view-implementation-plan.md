# Plan implementacji widoku Ekran informacji o użytkowniku

## 1. Przegląd
Widok ma za zadanie wyświetlić szczegółowe dane konta aktualnie zalogowanego użytkownika. Użytkownik powinien zobaczyć: imię, nazwisko, adres email oraz datę utworzenia konta. Widok ma charakter wyłącznie informacyjny (tylko do odczytu) i musi spełniać wymogi dotyczące przejrzystości, bezpieczeństwa oraz dostępności.

## 2. Routing widoku
- Ścieżka: `/user/info`

## 3. Struktura komponentów
- **UserInfoPage** – główny komponent strony, obsługuje pobieranie danych użytkownika z API oraz zarządza stanem widoku.
  - **UserInfoCard** – komponent prezentujący dane użytkownika w formie czytelnej karty/panelu.

## 4. Szczegóły komponentów
### UserInfoPage
- **Opis**: Strona, która przy inicjalizacji wywoła API endpoint `/users/me` w celu pobrania danych profilu użytkownika.
- **Główne elementy**:
  - Wywołanie API przy montowaniu komponentu.
  - Obsługa stanów: ładowanie, błąd, wyświetlenie danych.
  - Renderowanie komponentu UserInfoCard po otrzymaniu poprawnej odpowiedzi.
- **Obsługiwane interakcje**: Brak interakcji modyfikujących dane; ewentualna obsługa automatycznego przeładowania w przypadku błędów autoryzacji.
- **Warunki walidacji**:
  - Sprawdzanie, czy użytkownik jest autoryzowany.
  - Walidacja poprawności otrzymanych danych API.
- **Typy**: Korzysta z modelu danych odpowiadającego strukturze UserDto.
- **Propsy**: Brak – zarządza własnym stanem i przekazuje dane do komponentu dziecka.

### UserInfoCard
- **Opis**: Prezentuje w czytelnej formie dane użytkownika.
- **Główne elementy**:
  - Statyczne pola tekstowe: Imię, Nazwisko, Adres Email, Data utworzenia.
  - Prosty layout w formie karty lub panelu, wykorzystujący np. komponenty z biblioteki MudBlazor.
- **Obsługiwane interakcje**: Brak interakcji (widok tylko do odczytu).
- **Warunki walidacji**: Dane muszą zostać przekazane w odpowiednim formacie; pola powinny być wyświetlane tylko wtedy, gdy zawierają wartości.
- **Typy**: Przyjmuje obiekt typu UserViewModel (mapujący pola UserDto do widoku).
- **Propsy**: 
  - `UserData` – obiekt zawierający: Imię, Nazwisko, Email, Data utworzenia.

## 5. Typy
- **UserDto** (dostarczony w API): zawiera pola `id`, `email`, `first_name`, `last_name`, `created_at`.
- **UserViewModel** (typ dla widoku – może być aliasem dla UserDto lub mapperem):
  - `FirstName` (string)
  - `LastName` (string)
  - `Email` (string)
  - `CreatedAt` (DateTime lub string – odpowiednio sformatowana data)

## 6. Zarządzanie stanem
- **UserInfoPage** będzie zarządzał stanem lokalnym:
  - Zmienna stanu do przechowywania danych użytkownika.
  - Flagi: `isLoading`, `hasError`.
- Wykorzystanie mechanizmu asynchronicznego pobierania danych przez wstrzyknięty HttpClient.
- Brak potrzeby tworzenia niestandardowych hooków, wystarczą standardowe mechanizmy Blazor (np. `OnInitializedAsync`).

## 7. Integracja API
- **Endpoint**: GET `/users/me`
- **Żądanie**: Użycie HttpClient z nagłówkiem `Authorization: Bearer JWT_TOKEN`.
- **Odpowiedź**: Obiekt zgodny z UserDto.
- Integracja odbywa się w komponencie UserInfoPage, który po odebraniu danych przekazuje je do UserInfoCard.

## 8. Interakcje użytkownika
- Użytkownik przechodzi na adres `/user/info`, gdzie widok automatycznie pobiera dane z API.
- W przypadku braku autoryzacji (401) lub błędu serwera, wyświetlany jest komunikat o błędzie.
- Brak interakcji edycyjnych – widok tylko wyświetla informacje.

## 9. Warunki i walidacja
- Weryfikacja obecności tokenu autoryzacyjnego przed wykonaniem zapytania.
- Sprawdzenie poprawności odpowiedzi z API i odpowiednia obsługa błędów.
- Walidacja pola daty – sformatowanie daty w przyjazny dla użytkownika format.

## 10. Obsługa błędów
- Jeżeli API zwróci błąd (np. 401 Unauthorized lub 500), komponent wyświetla komunikat informujący użytkownika o problemie.
- Możliwość ponownego pobrania danych (np. przycisk "Odśwież" widoczny w przypadku błędu).

## 11. Kroki implementacji
1. Utworzenie pliku widoku: `/Pages/UserInfo.razor` z dyrektywą `@page "/user/info"`.
2. Wstrzyknięcie HttpClient i ewentualnej usługi autoryzacyjnej.
3. Implementacja logiki pobierania danych profilu w metodzie `OnInitializedAsync`.
4. Utworzenie komponentu `UserInfoCard.razor` w folderze np. `/Shared` lub `/Components` korzystającego z MudBlazor.
5. Przekazanie otrzymanych danych (UserViewModel) z UserInfoPage do UserInfoCard jako parametr.
6. Implementacja warunków ładowania i obsługi błędów (wyświetlanie spinnera, komunikatów o błędzie).
7. Testowanie widoku pod kątem poprawnego wyświetlania danych i obsługi błędów.
8. Weryfikacja zgodności widoku z zasadami UX, dostępności i bezpieczeństwa.
