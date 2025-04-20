# Architektura UI dla 10xCards

## 1. Przegląd struktury UI
Interfejs użytkownika MVP zostaje podzielony na kilka kluczowych widoków: ekran logowania, ekran rejestracji, dashboard (lista fiszek), ekran generowania fiszki, ekran edycji fiszki oraz sekcję logów błędów. Głównym elementem nawigacyjnym jest menu boczne, dostępne we wszystkich widokach po zalogowaniu, które umożliwia szybkie przechodzenie między funkcjonalnościami. Interfejs ma być stylowany przy użyciu MudBlazor i TailwindCSS, zapewniając responsywność dla urządzeń desktop, tablet i mobile.

## 2. Lista widoków

- **Ekran logowania**
  - **Ścieżka widoku:** `/login`
  - **Główny cel:** Umożliwienie użytkownikowi logowania do systemu.
  - **Kluczowe informacje do wyświetlenia:** Formularz zawierający pola do wprowadzenia adresu email oraz hasła.
  - **Kluczowe komponenty widoku:** Formularz logowania, przycisk akcji, dynamiczna walidacja inline.
  - **UX, dostępność i względy bezpieczeństwa:** Prosty interfejs z natychmiastową walidacją, zabezpieczony komunikacją HTTPS, bez dodatkowych elementów rozpraszających uwagę.

- **Ekran rejestracji**
  - **Ścieżka widoku:** `/register`
  - **Główny cel:** Umożliwienie nowym użytkownikom utworzenia konta.
  - **Kluczowe informacje do wyświetlenia:** Formularz z polami: email, imię, nazwisko, hasło.
  - **Kluczowe komponenty widoku:** Formularz rejestracji, przycisk "Zarejestruj się", dynamiczna walidacja inline.
  - **UX, dostępność i względy bezpieczeństwa:** Intuicyjny formularz, jasne komunikaty błędów (toast notifications) oraz bezpieczeństwo danych zgodnie z wymaganiami.

- **Dashboard (lista fiszek)**
  - **Ścieżka widoku:** `/dashboard`
  - **Główny cel:** Prezentacja listy fiszek użytkownika.
  - **Kluczowe informacje do wyświetlenia:** 
    - Skrócony fragment frontu fiszki (pierwsze 150 znaków),
    - Typ generowania fiszki (AI lub Human),
    - Data utworzenia oraz data ostatniej edycji.
  - **Kluczowe komponenty widoku:** Lista lub karta fiszek, przyciski umożliwiające przejście do edycji/detalów.
  - **UX, dostępność i względy bezpieczeństwa:** Przejrzysta prezentacja danych z dynamicznym ładowaniem na żądanie oraz możliwość sortowania i filtrowania (wg wymagań sortowanie domyślne od najnowszych).

- **Ekran generowania fiszki**
  - **Ścieżka widoku:** `/cards/generate`
  - **Główny cel:** Umożliwienie generacji fiszki przy użyciu AI na podstawie wprowadzonego tekstu.
  - **Kluczowe informacje do wyświetlenia:** 
    - Pole tekstowe do wprowadzenia treści źródłowej (1000–10000 znaków),
    - Pola wyświetlające wygenerowany front i tył fiszki.
  - **Kluczowe komponenty widoku:** Formularz generowania – pole do wprowadzenia treści, przycisk "Generuj" (wywołanie AI), przycisk "Zapisz" do zatwierdzenia wygenerowanej fiszki, dynamiczna aktualizacja pola.
  - **UX, dostępność i względy bezpieczeństwa:** Intuicyjna obsługa, dynamiczna walidacja inline oraz informowanie użytkownika o błędach poprzez toast notifications.

- **Ekran edycji fiszki**
  - **Ścieżka widoku:** `/cards/edit/{id}`
  - **Główny cel:** Umożliwienie edycji już zapisanej fiszki.
  - **Kluczowe informacje do wyświetlenia:** Obecne wartości pól fiszki (front, back) umożliwiające użytkownikowi modyfikację.
  - **Kluczowe komponenty widoku:** Formularz edycji, przyciski do zapisywania modyfikacji, dynamiczna walidacja.
  - **UX, dostępność i względy bezpieczeństwa:** Prosty interfejs umożliwiający natychmiastową edycję z natychmiastową walidacją oraz bezpieczeństwem operacji.

- **Ekran logów błędów**
  - **Ścieżka widoku:** `/cards/errors`
  - **Główny cel:** Udostępnienie użytkownikowi możliwości przeglądania logów błędów związanych z generacją fiszek.
  - **Kluczowe informacje do wyświetlenia:** Data powstania błędu oraz oryginalna treść, z domyślnym sortowaniem od najnowszego błędu.
  - **Kluczowe komponenty widoku:** Tabela lub lista logów, elementy wyświetlające daty i treść błędów.
  - **UX, dostępność i względy bezpieczeństwa:** Czytelny podgląd błędów z łatwym dostępem przez menu boczne, brak dodatkowych filtrów w MVP.

## 3. Mapa podróży użytkownika
1. **Logowanie/Rejestracja:** Użytkownik rozpoczyna od ekranu logowania `/login` lub, w przypadku nowych użytkowników, przechodzi do rejestracji `/register`.
2. **Dashboard:** Po pomyślnym zalogowaniu użytkownik trafia do dashboardu `/dashboard`, gdzie przegląda listę zapisanych fiszek.
3. **Generowanie fiszki:** Użytkownik wybiera opcję generowania fiszki z menu bocznego, przechodząc do ekranu `/cards/generate`. Tam wprowadza treść źródłową, klika przycisk "Generuj", aby AI wygenerowało front i tył, a następnie zapisuje wynik przyciskiem "Zapisz". Po udanym zapisie fiszka pojawia się w dashboardzie.
4. **Edycja fiszki:** Użytkownik może wybrać pojedynczą fiszkę z dashboardu, aby przejść do ekranu edycji `/cards/edit/{id}`, gdzie dokonuje zmian i zapisuje aktualizacje.
5. **Logi błędów:** Użytkownik ma możliwość przejścia do sekcji logów błędów, gdzie przegląda listę błędów dotyczących generacji fiszek, sortowaną od najnowszych.

## 4. Układ i struktura nawigacji
- **Menu boczne:** Główny system nawigacji dostępny po zalogowaniu, zawiera odnośniki do:
  - Dashboard (lista fiszek)
  - Ekranu generowania fiszki
  - Ekranu rejestracji/edycji (opcjonalnie dostęp do konta)
  - Sekcji logów błędów
  - Opcji wylogowania
- **Adaptacja mobilna:** Na urządzeniach mobilnych menu boczne będzie wyświetlane jako rozwijane menu (hamburger menu) z zachowaniem responsywności.
- **Nawigacja kontekstowa:** W dashboardzie umożliwiony jest szybki dostęp do edycji pojedynczych fiszek przez kliknięcie w element listy.

## 5. Kluczowe komponenty
- **Formularze:** Formularze logowania, rejestracji, generowania oraz edycji fiszek z dynamiczną walidacją inline.
- **Lista/Karty fiszek:** Komponent prezentujący skrócony podgląd fiszek (150 znaków front, typ generacji i daty).
- **Toast notifications:** Komponent komunikatów wyświetlających błędy lub potwierdzenie akcji.
- **Komponent menu bocznego:** Element nawigacyjny utrzymujący spójny dostęp do głównych widoków.
- **Komponent logów błędów:** Tabela lub lista wyświetlająca logi błędów zgodnie z wymaganiami (data oraz oryginalna treść, sortowane od najnowszego).
