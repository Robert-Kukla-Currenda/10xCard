# Stos technologiczny

## 1. Frontend
- framework Blazor:
- MudBlazor jako biblioteka componentów wizualnych
- Framework TailwindCSS zapewnia proste i szybkie stylowanie

## 2. Backend 
- .NET 8 jako stabilna i łatwa platforma umożliwiająca budowanie wszechstronnych aplikacji.
- Microsoft Entity Framework: ORM (Object-Relational Mapping) dla .NET, umożliwiający łatwe mapowanie obiektów na tabele w bazie danych PostgreSQL.
- Baza danych: Postgres 17.


## 3. AI
Komunikacja z modelami przez usługę Openrouter.ai:
Dostęp do szerokiej gamy modeli (OpenAI, Anthropic, Google i wiele innych), które pozwolą nam znaleźć rozwiązanie zapewniające wysoką efektywność i niskie koszta
Pozwala na ustawianie limitów finansowych na klucze API

## 4. CI/CD
CI/CD i Hosting:
Github Actions do tworzenia pipeline'ów CI/CD
DigitalOcean do hostowania aplikacji za pośrednictwem obrazu docker

## 5. Narzędzia testowe
### Testy jednostkowe
- xUnit jako główny framework testowy
- FluentAssertions do czytelnych asercji
- TestContainers do testów integracyjnych z bazą danych PostgreSQL
- WireMock.NET do mockowania zewnętrznych API
- AutoFixture do generowania danych testowych
- Coverlet do mierzenia pokrycia kodu testami

### Testy E2E
- Playwright do testów end-to-end (zamiast Selenium)
- bUnit do testowania komponentów Blazor
- Axe dla testów dostępności
- NBomber jako narzędzie do testów obciążeniowych

### Dodatkowe narzędzia
- Storybook do izolowanego testowania komponentów UI
- OWASP ZAP do automatycznego skanowania podatności
- SonarQube z pluginem Security do statycznej analizy bezpieczeństwa
- Snyk do monitorowania podatności w zależnościach
- Application Insights do monitorowania wydajności
