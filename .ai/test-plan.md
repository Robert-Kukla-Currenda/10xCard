## Plan Testów dla projektu 10xCards

### 1. Wprowadzenie i cele testowania
Celem testowania jest zapewnienie, że aplikacja 10xCards działa zgodnie ze specyfikacją, spełnia wymagania jakościowe oraz jest bezpieczna i wydajna. Testy mają na celu wykrycie błędów na wczesnym etapie, co pozwoli usprawnić proces wdrażania i utrzymania aplikacji zgodnie z zasadami SOLID i czystego kodu.

### 2. Zakres testów
- **Backend (TenXCards.API):** Testy endpointów API (.NET 8, Entity Framework, PostgreSQL 17) w zakresie rejestracji, logowania, zarządzania fiszkami, integracji z usługą Openrouter.ai oraz obsługi błędów.
- **Frontend (TenXCards.Frontend):** Testy komponentów Blazor, interakcji użytkownika, walidacji formularzy, integracji z API, wizualnych elementów wykorzystujących MudBlazor i TailwindCSS.
- **Integracja:** Testy przepływu danych między warstwą frontend i backend oraz poprawności autoryzacji przy wykorzystaniu JWT.
- **Wydajność i bezpieczeństwo:** Testy obciążeniowe oraz testy bezpieczeństwa kluczowych endpointów (np. logowanie, tworzenie treści) oraz testy ochrony przed nadużyciami.

### 3. Typy testów
- **Testy jednostkowe:** Weryfikacja pojedynczych metod i funkcji, np. walidacja modeli, logika biznesowa w serwisach zgodnie z zasadami separacji warstw.
- **Testy integracyjne:** Testowanie współdziałania pomiędzy komponentami backendu, integracja API z bazą danych, przepływ autoryzacji.
- **Testy end-to-end (E2E):** Symulacja scenariuszy użytkownika w interfejsie Blazor, obejmujące pełny cykl działania (np. rejestracja, logowanie, operacje CRUD na fiszkach).
- **Testy wydajnościowe:** Ocena obciążenia endpointów API oraz frontend pod kątem czasu odpowiedzi przy równoczesnych żądaniach.
- **Testy bezpieczeństwa:** Sprawdzenie odporności na ataki (np. SQL Injection, XSS) oraz poprawności implementacji mechanizmu autoryzacji JWT.
- **Testy dostępności:** Weryfikacja zgodności z wymogami dostępności WCAG dla komponentów interfejsu użytkownika.

### 4. Scenariusze testowe dla kluczowych funkcjonalności
- **Rejestracja użytkownika:**
  - Weryfikacja walidacji danych wejściowych.
  - Testy pozytywne dla poprawnych danych i negatywne dla błędnych formatów (np. nieprawidłowy email).
  - Testowanie zabezpieczeń przed atakami typu brute-force.
- **Logowanie użytkownika:**
  - Test poprawnego działania autoryzacji (200 – Success).
  - Obsługa błędnych danych (401 – Unauthorized).
  - Weryfikacja poprawności generowania i walidacji tokenów JWT.
- **Operacje CRUD na fiszkach:**
  - Tworzenie, edycja, usuwanie oraz pobieranie kart.
  - Testy integracyjne sprawdzające przepływ danych między API a bazą danych.
  - Weryfikacja poprawności paginacji i sortowania wyników.
- **Integracja z usługą Openrouter.ai:**
  - Test poprawności komunikacji z usługą AI, walidacja otrzymanych odpowiedzi oraz retry logic w przypadku błędów połączenia.
  - Weryfikacja mechanizmów limitowania kosztów API zgodnie z wymaganiami.
- **Interfejs użytkownika (frontend):**
  - Walidacja formularzy z wykorzystaniem MudBlazor i TailwindCSS.
  - Testy interakcji, np. komunikaty toast, dynamiczne walidacje inline, paginacja na widoku Dashboard.
  - Weryfikacja responsywności interfejsu na różnych urządzeniach.
- **Obsługa błędów i bezpieczeństwo:**
  - Test scenariuszy nieautoryzowanego dostępu.
  - Weryfikacja logowania błędów oraz poprawności komunikatów zwrotnych.
  - Testy zabezpieczeń przed CSRF, XSS i innymi popularnymi podatnościami.

### 5. Środowisko testowe
- **Lokalne środowisko deweloperskie:** Uruchamiane na maszynie Windows z wykorzystaniem Visual Studio Code.
- **Środowisko CI/CD:** GitHub Actions do automatycznego budowania, testowania i wdrażania w środowisku staging.
- **Baza danych:** TestContainers dla PostgreSQL zapewniające izolowane środowisko testowe zgodne z konfiguracją w `appsettings.json`.
- **Symulacja API:** WireMock.NET do mockowania zewnętrznych usług, w tym Openrouter.ai.
- **Wirtualizacja środowiska:** Docker do zapewnienia spójności między środowiskami testowymi.

### 6. Narzędzia do testowania
- **Backend:** 
  - xUnit jako główny framework testowy
  - FluentAssertions do czytelnych asercji
  - TestContainers do testów integracyjnych z bazą danych
  - WireMock.NET do mockowania zewnętrznych API
  - AutoFixture do generowania danych testowych
- **Frontend:** 
  - bUnit do testowania komponentów Blazor
  - Playwright do testów E2E (zamiast Selenium)
  - Storybook do izolowanego testowania komponentów UI
  - Axe dla testów dostępności
- **Wydajnościowe:** 
  - NBomber jako narzędzie oparte na .NET do testów obciążeniowych
  - Application Insights do monitorowania wydajności
- **Bezpieczeństwa:** 
  - OWASP ZAP do automatycznego skanowania podatności
  - SonarQube z pluginem Security do statycznej analizy bezpieczeństwa
  - Snyk do monitorowania podatności w zależnościach
- **CI/CD:** 
  - GitHub Actions z integracją raportowania wyników testów
  - Coverlet do mierzenia pokrycia kodu testami

### 7. Harmonogram testów
- **Faza 1 – Testy jednostkowe:** Realizowane równolegle z rozwojem komponentów, automatycznie uruchamiane przy każdym puszcie na branch.
- **Faza 2 – Testy integracyjne:** Uruchamiane po zakończeniu kluczowych modyfikacji backend i frontend.
- **Faza 3 – Testy E2E i wydajnościowe:** Przeprowadzane przed wdrożeniem na środowisko staging.
- **Faza 4 – Testy bezpieczeństwa:** Regularne skanowanie co wydanie oraz przed produkcyjnym wdrożeniem.
- **Testowanie ciągłe:** Kluczowe testy uruchamiane automatycznie w ramach pipeline CI/CD.

### 8. Kryteria akceptacji testów
- Wszystkie testy jednostkowe i integracyjne muszą mieć wynik PASS.
- Testy E2E nie powinny wykazać błędów krytycznych blokujących funkcjonalność.
- Czas odpowiedzi kluczowych endpointów nie przekracza ustalonych limitów (200ms dla operacji odczytu, 500ms dla operacji zapisu).
- Brak podatności wykrytych podczas testów bezpieczeństwa na poziomie krytycznym.
- Pokrycie kodu testami jednostkowymi minimum 80% dla logiki biznesowej.
- Zgodność z wymogami dostępności WCAG 2.1 poziom AA dla głównych funkcji interfejsu.

### 9. Role i odpowiedzialności
- **Inżynier QA:** Opracowanie i utrzymanie planu testów, pisanie testów jednostkowych, integracyjnych i E2E, raportowanie błędów.
- **Deweloperzy:** Wdrażanie poprawek zgłoszonych przez QA, uczestnictwo w code review oraz wsparcie przy tworzeniu testów jednostkowych zgodnie z podejściem TDD.
- **Testerzy manualni:** Przeprowadzanie testów manualnych scenariuszy użytkownika, walidacja interfejsu graficznego, testy regresyjne.
- **Manager projektu:** Koordynacja harmonogramu testów oraz monitorowanie kryteriów akceptacji.
- **DevOps:** Utrzymanie środowiska CI/CD i zapewnienie automatyzacji procesów testowych.

### 10. Procedury raportowania błędów
- **Narzędzia:** GitHub Issues jako główne narzędzie do śledzenia błędów, zintegrowane z procesem CI/CD.
- **Zgłoszenie błędu:** Każdy błąd musi zawierać szczegółowy opis, kroki do reprodukcji, oczekiwany rezultat oraz rzeczywisty wynik, a także zrzuty ekranu lub logi jeśli to możliwe.
- **Priorytetyzacja:** Błędy klasyfikowane jako:
  - **Krytyczne:** Uniemożliwiające działanie kluczowych funkcjonalności.
  - **Wysokie:** Poważnie wpływające na doświadczenie użytkownika.
  - **Średnie:** Problemy z funkcjonalnością wymagające obejścia.
  - **Niskie:** Drobne problemy kosmetyczne lub sugestie ulepszeń.
- **Śledzenie:** Regularne przeglądy zgłoszonych błędów, weryfikacja ich naprawy przez zespół developerski oraz aktualizacja statusu w GitHub Issues.
- **Automatyzacja:** Integracja zgłoszeń z systemem CI/CD, automatyczne przydzielanie zadań i śledzenie statusu.

### 11. Dodatkowe praktyki testowe
- **Property-based testing:** Wykorzystanie FsCheck do automatycznego generowania scenariuszy testowych.
- **Contract testing:** Implementacja Pact dla zapewnienia zgodności API między frontendem a backendem.
- **Snapshot testing:** Dla komponentów UI w celu wykrywania niezamierzonych zmian wizualnych.
- **Visual regression testing:** Percy do automatycznego wykrywania zmian wizualnych w interfejsie.
- **Testowanie dostępności:** Regularne audyty dostępności z wykorzystaniem narzędzi automatycznych i manualnych.

--- 

Ten plan testów stanowi kompleksowy dokument, który ma za zadanie zapewnić wysoką jakość aplikacji TenXCards przy jednoczesnym wykorzystaniu najnowszych praktyk testowych i specyfiki wykorzystywanych technologii zgodnie z zasadami czystego kodu i architekturalną strukturą projektu.