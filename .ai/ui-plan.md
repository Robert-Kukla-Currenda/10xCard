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

- **Ekran informacji o użytkowniku**
  - **Ścieżka widoku:** `/user/info`
  - **Główny cel:** Wyświetlenie szczegółowych danych konta użytkownika.
  - **Kluczowe informacje do wyświetlenia:** Imię, nazwisko, adres email, data utworzenia.
  - **Kluczowe komponenty widoku:** Panel lub karta prezentująca dane użytkownika, tylko do odczytu.
  - **UX, dostępność i względy bezpieczeństwa:** Przejrzysty układ, brak możliwości edycji, odpowiednie zabezpieczenia przy wyświetlaniu danych.

- **Dashboard (lista fiszek)**
  - **Ścieżka widoku:** `/dashboard`
  - **Główny cel:** Prezentacja listy treści źródłowych, z których zostały wygenerowane fiszki, oraz umożliwienie przeglądania powiązanych fiszek.
  - **Kluczowe informacje do wyświetlenia:** 
    - Lista treści źródłowych,
    - Dla każdej treści, lista powiązanych fiszek zawierająca skrócony fragment frontu (150 znaków), typ generacji, daty utworzenia oraz daty ostatniej edycji.
  - **Kluczowe komponenty widoku:** Komponent listy treści z rozwijalnymi sekcjami prezentującymi powiązane fiszki oraz przyciski umożliwiające przejście do edycji/detalów.
  - **UX, dostępność i względy bezpieczeństwa:** Intuicyjna nawigacja między treściami i powiązanymi fiszkami, dynamiczne ładowanie na żądanie oraz możliwość sortowania i filtrowania (domyślnie sortowanie od najnowszych).

- **Modal tworzenia/dodawania fiszki do istniejącej treści**
  - **Lokalizacja w interfejsie:** Implementowany jako modal na widoku `/dashboard`
  - **Główny cel:** Umożliwienie ręcznego dodania fiszki do treści, wywoływanych bezpośrednio z dashboardu.
  - **Kluczowe informacje do wyświetlenia:** 
    - Pola tekstowe do wprowadzenia fiszki. Front - maksymalnie 1000 znaków, back - maksymalnie 5000 znaków,    
    - Informacja o możliwości dodania wielu fiszek do jednej treści.
  - **Kluczowe komponenty widoku:** Formularz dodawania fiszki jako modal, z dynamiczną aktualizacją pól i przyciskiem "Zapisz".
  - **UX, dostępność i względy bezpieczeństwa:** Intuicyjna obsługa z natychmiastową walidacją inline i komunikatami błędów (toast notifications), dostępna w ramach dashboardu.

- **Modal edycji fiszki**
  - **Lokalizacja w interfejsie:** Wyświetlany jako modal na widoku `/dashboard` po wybraniu fiszki do edycji
  - **Główny cel:** Umożliwienie edycji zapisanej fiszki, wywoływanego z poziomu dashboardu.
  - **Kluczowe informacje do wyświetlenia:** Obecne wartości pól fiszki (front, back) do modyfikacji.
  - **Kluczowe komponenty widoku:** Formularz edycji jako modal z przyciskami zapisania modyfikacji oraz dynamiczną walidacją.
  - **UX, dostępność i względy bezpieczeństwa:** Prosty, natychmiastowy interfejs edycji z wbudowaną walidacją, dostępny bez opuszczania widoku dashboardu.

- **Ekran logów błędów - opcjonalny**
  - **Ścieżka widoku:** `/cards/errors`
  - **Główny cel:** Udostępnienie użytkownikowi możliwości przeglądania logów błędów związanych z generacją fiszek.
  - **Kluczowe informacje do wyświetlenia:** Data powstania błędu oraz oryginalna treść, z domyślnym sortowaniem od najnowszego błędu.
  - **Kluczowe komponenty widoku:** Tabela lub lista logów, elementy wyświetlające daty i treść błędów.
  - **UX, dostępność i względy bezpieczeństwa:** Czytelny podgląd błędów z łatwym dostępem przez menu boczne, brak dodatkowych filtrów w MVP.

## 3. Mapa podróży użytkownika
1. **Logowanie/Rejestracja:**  
   Użytkownik rozpoczyna od ekranu logowania `/login` lub, w przypadku nowych użytkowników, przechodzi do rejestracji `/register`. Proces ten zapewnia bezpieczny dostęp oraz umożliwia zarządzanie treściami edukacyjnymi na poziomie konta użytkownika.
2. **Dashboard:**  
   Po pomyślnym zalogowaniu użytkownik trafia do dashboardu `/dashboard`, gdzie widzi listę treści źródłowych. Dla każdej treści wyświetlana jest lista powiązanych fiszek – zarówno tych wygenerowanych automatycznie przez AI, jak i stworzonych manualnie – wraz z ich skróconymi fragmentami, typem generacji oraz datami utworzenia i edycji.
3. **Generowanie fiszki:**  
   Użytkownik wybiera opcję generowania fiszki z menu bocznego, przechodząc do ekranu `/cards/generate`. Na tym etapie możliwe jest:
      - Wprowadzenie treści źródłowej i użycie AI do automatycznego generowania fiszek przy użyciu technik streszczenia i redakcji,
      - Ręczne dodanie nowej fiszki poprzez interfejs „Dodaj nową fiszkę”.  
   W obu przypadkach, wygenerowana lub stworzona fiszka jest powiązana z treścią bazową, a użytkownik zostaje umożliwiony jej dalszą edycję.
4. **Edycja/Usuwanie fiszki:**  
   Użytkownik może wybrać pojedynczą fiszkę z dashboardu, przechodząc do ekranu edycji `/cards/edit/{id}`, gdzie może ją modyfikować, zapisywać zmiany lub usuwać – niezależnie od tego, czy fiszka została wygenerowana przez AI, czy stworzona ręcznie.
5. **Logi błędów:**  
   W przypadku wystąpienia problemów podczas generowania fiszek, użytkownik ma dostęp do sekcji logów błędów (`/cards/errors`), gdzie przegląda listę błędów (sortowaną od najnowszych) oraz ich szczegółowe opisy.

## 4. Układ i struktura nawigacji
- **Menu boczne:** Główny system nawigacji dostępny po zalogowaniu, zawiera odnośniki do:
  - Dashboard (lista fiszek)
  - Ekranu generowania fiszki
  - Ekranu informacji o koncie użytkownika (imię, nazwisko, adres email, datę utworzenia) - brak możliwości edycji
  - Sekcji logów błędów
  - Opcji wylogowania
- **Adaptacja mobilna:** Na urządzeniach mobilnych menu boczne będzie wyświetlane jako rozwijane menu (hamburger menu) z zachowaniem responsywności.

## 5. Kluczowe komponenty
- **Formularze:** Formularze logowania, rejestracji, generowania oraz edycji fiszek z dynamiczną walidacją inline.
- **Lista/Karty fiszek:** Komponent prezentujący skrócony podgląd fiszek (150 znaków front, typ generacji i daty).
- **Toast notifications:** Komponent komunikatów wyświetlających błędy lub potwierdzenie akcji.
- **Komponent menu bocznego:** Element nawigacyjny utrzymujący spójny dostęp do głównych widoków.
- **Komponent logów błędów:** Tabela lub lista wyświetlająca logi błędów zgodnie z wymaganiami (data oraz oryginalna treść, sortowane od najnowszego).
