# Plan implementacji widoku Dashboard

## 1. Przegląd
Widok **Dashboard** służy do prezentacji listy treści źródłowych oraz zagnieżdżonych pod nimi fiszek. Umożliwia przeglądanie, dodawniae, edycję i usuwanie fiszek w sposób intuicyjny i zgodny z wymaganiami dotyczącymi zarządzania materiałami edukacyjnymi.

## 2. Routing widoku
Widok będzie dostępny pod ścieżką:
- `/dashboard`

## 3. Struktura komponentów
Poniżej przedstawiono przykładową hierarchię:

```
DashboardView
└── OriginalContentsList
    ├── OriginalContentItem
    │   └── FiszkiList
    │       └── FiszkaItem
    └── Paginator (opcjonalnie)
```

## 4. Szczegóły komponentów

### DashboardView
- **Opis:** Główny komponent widoku, odpowiedzialny za pobieranie danych o treściach źródłowych i ich fiszkach oraz renderowanie listy.
- **Główne elementy:**  
  - Nagłówek z tytułem widoku.  
  - Komponent z listą treści (OriginalContentsList).  
  - Przycisk/przejście na inne widoki lub funkcje (np. do `CreateContentView`).
- **Obsługiwane interakcje:** Ładowanie danych z API podczas montowania, obsługa błędów.
- **Warunki walidacji:** Brak bezpośrednich warunków – walidacja odbywa się na kolejnych komponentach lub w trakcie komunikacji z API.
- **Typy:** Może przyjmować i przekazywać dalej kolekcję obiektów `OriginalContentDto`.
- **Propsy:** Brak specjalnych, bo sam zarządza stanem i zwraca gotowe dane do dzieci.

### OriginalContentsList
- **Opis:** Renderuje listę treści źródłowych z możliwością rozwinięcia do powiązanych fiszek.
- **Główne elementy:**  
  - Lista pozycji (OriginalContentItem).  
  - Kontrolki paginacji (Paginator) i sortowania.
- **Obsługiwane interakcje:**  
  - Zmiana strony (paginacja).  
  - Zmiana kolejności sortowania.  
  - Rozwijanie/zwijanie list powiązanych fiszek.
- **Warunki walidacji:** Niezbędne sprawdzenie poprawności parametrów wyszukiwania i paginacji.
- **Typy:** Przyjmuje tablicę `OriginalContentDto` i informacje o paginacji.
- **Propsy:**  
  - `originalContents: OriginalContentDto[]`  
  - `paginationInfo` (np. obiekt zawierający `page`, `limit`, `total`).

### OriginalContentItem
- **Opis:** Wyświetla pojedynczą treść źródłową, a poniżej listę fiszek (FiszkiList).
- **Główne elementy:**  
  - Nagłówek z kluczowymi danymi (np. fragment tekstu źródłowego, data).  
  - Sekcja przechowująca listę fiszek.
- **Obsługiwane interakcje:**  
  - Rozwijanie lub zwijanie sekcji z fiszkami. 
  - Dodawanie nowej fiszki do treści.
  - Usuwanie treści wraz z powiązanymi fiszkami.
  - Możliwość przejścia do edycji treści (opcjonalnie).
- **Warunki walidacji:** Brak dodatkowych – walidacja danych występuje poza zakresem komponentu.
- **Typy:** `OriginalContentDto` z polami identyfikującymi treść i listę fiszek.
- **Propsy:**  
  - `item: OriginalContentDto`

### FiszkiList
- **Opis:** Wyświetla listę powiązanych fiszek. Pozwala na edycję, usuwanie i przeglądanie szczegółów.
- **Główne elementy:**  
  - Lista komponentów FiszkaItem.  
  - Kontrolki wywołujące czynności (np. usuwanie).
- **Obsługiwane interakcje:**  
  - Usuwanie i edycja fiszki.  
  - Aktualizacja widoku w razie zmiany w API.
- **Warunki walidacji:** Sprawdzanie dostępności danych fiszki (front/back).
- **Typy:** Może pobierać tablicę `CardDto`.
- **Propsy:**  
  - `cards: CardDto[]`
  - Funkcje do obsługi edycji i usuwania (np. `onEdit(cardId)`, `onDelete(cardId)`).

### FiszkaItem
- **Opis:** Wizualizuje pojedynczą fiszkę i jej dane (skrócony front, data, typ generacji).
- **Główne elementy:**  
  - Wyświetlenie fragmentu frontu (max 150 znaków).  
  - Informacja o dacie utworzenia i ostatniej edycji.
- **Obsługiwane interakcje:**  
  - Kliknięcie w przycisk „Edytuj” czy „Usuń”.
- **Warunki walidacji:** Sprawdzenie czy pole `front` i `back` są prawidłowo ograniczone do maksymalnej długości.
- **Typy:** `CardDto` z kluczowymi polami: `front`, `back`, `generated_by` itd.
- **Propsy:**  
  - `card: CardDto`  
  - `onEdit`, `onDelete`

## 5. Typy
- **OriginalContentDto** – zawiera pola takie jak `id`, `userId`, `content`, `createdAt` i listę `cards`.
- **CardDto** – pola `id`, `front`, `back`, `generatedBy`, `createdAt`.
- **PaginationDto** (opcjonalnie) – może zawierać `page`, `limit`, `total`.
- **View Models** – dodatkowe typy do obróbki stanu sortowania i paginacji, np. `DashboardState`, zawierające:
  - `currentPage: number`  
  - `sortOrder: string`  
  - `contents: OriginalContentDto[]`

## 6. Zarządzanie stanem
- Stanem zarządza główny komponent `DashboardView`. Trzyma informacje o:
  - Aktualnej stronie (`currentPage`), sortowaniu (`sortOrder`), liście treści (`originalContents`).
- W razie potrzeby można wprowadzić kontekst (np. `DashboardContext`) lub dedykowany hook do ponownego użycia logiki pobierania danych.

## 7. Integracja API
- **Endpoint:** `GET /api/originalcontents`  
  - Zwraca listę obiektów `OriginalContentDto` wraz z polami `cards`.
  - Parametry: `page`, `limit`, `sort` (opcjonalne).
- **Żądanie:** W nagłówku `Authorization: Bearer JWT_TOKEN_HERE`.  
- **Odpowiedź:** Zwraca strukturę JSON zawierającą `items` (tablica `OriginalContentDto`) oraz `pagination`.
- Przy usuwaniu lub edycji fiszek należy wywołać istniejące endpointy (`CardsController`).

## 8. Interakcje użytkownika
1. Użytkownik wchodzi na `/dashboard`:  
   - Komponent pobiera dane o treściach (z uwzględnieniem paginacji i sortowania).  
   - Wyświetla listę treści wraz z fiszkami w postaci rozwijalnej.
2. Kliknięcie rozwiń/zwijaj: pokazuje/zamyka listę fiszek dla danej treści.
3. Kliknięcie „Edytuj” (na FiszkaItem): prowadzi do formularza edycji w innym widoku lub wywołuje okno modalne.
4. Kliknięcie „Usuń” (na FiszkaItem): wysyła żądanie do API, po czym usuwa pozycję z listy na widoku.

## 9. Warunki i walidacja
- Sprawdzanie poprawności odpowiedzi z API (obsługa błędów przy braku tokena – 401).  
- Maksymalny limit znaków `front` (1000) i `back` (5000) jest już wymuszany przez API, ale można dodatkowo weryfikować w logice edycji fiszki.

## 10. Obsługa błędów
- Nieautoryzowany dostęp (401): Przekierowanie do widoku logowania.  
- Brak danych lub błąd komunikacji z API (5xx): Wyświetlenie komunikatu o błędzie i przycisku do ponownego załadowania.  
- Walidacja treści fiszki (4xx): Wyświetlenie powiadomienia dla użytkownika (np. toast).

## 11. Kroki implementacji
1. **Utwórz** komponent `DashboardView` z logiką pobierania listy treści.  
2. **Dodaj** wewnętrzny stan do zarządzania paginacją i sortowaniem.  
3. **Stwórz** komponent `OriginalContentsList` i zagnieżdżone `OriginalContentItem`.  
4. **Utwórz** komponent `FiszkiList` i `FiszkaItem` z obsługą przycisków edycji i usuwania.  
5. **Zaimplementuj** funkcję do realizacji żądania `GET /api/originalcontents` (uwzględnij token w nagłówku).  
6. **Zaimplementuj** obsługę edycji/usuwania (wywołania metod w `CardsController`).  
7. **Zintegruj** paginację i sortowanie (aktualizacja parametrów w zapytaniu do endpointu).  
9. **Zaimplementuj** wyświetlanie komunikatów błędów i potwierdzeń.  