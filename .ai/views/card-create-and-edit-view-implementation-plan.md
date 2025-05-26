# Plan implementacji widoku Modal Fiszek

## 1. Przegląd
Widok składa się z dwóch modali uruchamianych z dashboardu:  
- **Modal tworzenia/dodawania fiszki** – umożliwia ręczne dodanie nowej fiszki do istniejącej treści.  
- **Modal edycji fiszki** – pozwala na modyfikację danych już zapisanej fiszki.  
Celem obu modali jest zapewnienie intuicyjnego, responsywnego interfejsu z natychmiastową walidacją i komunikatami o błędach.

## 2. Routing widoku
Oba modale będą dostępne na widoku `/dashboard` i będą wywoływane jako nakładki (overlays) bez zmiany głównej ścieżki routingu.

## 3. Struktura komponentów
- **DashboardPage** (widok główny)  
  ├── **CreateFlashcardModal** (modal dodawania fiszki)  
  └── **EditFlashcardModal** (modal edycji fiszki)

## 4. Szczegóły komponentów

### CreateFlashcardModal
- **Opis:**  
  Modal umożliwiający ręczne dodanie fiszki do wybranej treści. Obejmuje pola tekstowe dla frontu (max 1000 znaków) oraz backu (max 5000 znaków). Informuje użytkownika o możliwości dodania wielu fiszek.
- **Główne elementy:**  
  - Formularz z dwoma polami tekstowymi  
  - Przycisk „Zapisz”  
  - Dynamiczne wyświetlanie komunikatów błędów (inline oraz toast notifications)
- **Obsługiwane interakcje:**  
  - Wprowadzanie danych w polach tekstowych  
  - Kliknięcie przycisku zapisania (wywołanie funkcji submit)
  - Zamykanie modalu (przycisk/cross)
- **Warunki walidacji:**  
  - Pole „front” – wymagane, długość między 1 a 1000 znaków  
  - Pole „back” – wymagane, długość między 1 a 5000 znaków  
- **Typy:**  
  - Model formularza: { front: string, back: string, originalContentId: number, generatedBy: string (na stałe „human”) }
- **Propsy:**  
  - isOpen (boolean) – kontrola widoczności modalu  
  - onClose – funkcja zamykająca modal  
  - originalContentId – identyfikator treści, do której dodajemy fiszkę

### EditFlashcardModal
- **Opis:**  
  Modal służący do edycji już utworzonej fiszki. Pola są wstępnie uzupełnione aktualnymi danymi, umożliwiającymi ich modyfikację.
- **Główne elementy:**  
  - Formularz z pre-populowanymi polami „front” i „back”  
  - Przycisk „Zapisz zmiany”  
  - Dynamiczna walidacja i komunikaty błędów
- **Obsługiwane interakcje:**  
  - Modyfikacja danych w polach tekstowych  
  - Kliknięcie przycisku zatwierdzającego edycję  
  - Zamykanie modalu (przycisk/cross)
- **Warunki walidacji:**  
  - Pole „front” – wymagane, długość między 1 a 1000 znaków  
  - Pole „back” – wymagane, długość między 1 a 5000 znaków  
- **Typy:**  
  - Model formularza: { id: number, front: string, back: string }
- **Propsy:**  
  - isOpen (boolean) – kontrola widoczności modalu  
  - onClose – funkcja zamykająca modal  
  - initialData – obiekt zawierający dane fiszki do edycji

## 5. Typy
- **FlashcardCreateModel (ViewModel do CreateFlashcardModal):**  
  - originalContentId: number  
  - front: string  
  - back: string  
  - generatedBy: string (ustawione na "human")
- **FlashcardEditModel (ViewModel do EditFlashcardModal):**  
  - id: number  
  - front: string  
  - back: string

Typy te odpowiadają polom wymaganym przez endpointy:  
- POST `/cards` – korzysta z FlashcardCreateModel  
- PUT `/cards/{id}` – korzysta z FlashcardEditModel

## 6. Zarządzanie stanem
- **Lokalny stan komponentów:**  
  W obu modalach użyjemy hooków useState do przechowywania wartości pól formularza oraz komunikatów walidacyjnych.
- **Custom Hook (opcjonalnie):**  
  Można stworzyć hooka `useFlashcardForm` do obsługi logiki walidacji i zmiany stanu, który będzie współdzielony przez oba modale.

## 7. Integracja API
- **Tworzenie fiszki:**  
  - Metoda: POST `/cards`  
  - Payload: `{ front, back, generated_by: "human", original_content_id }`  
  - Po udanej odpowiedzi – wyświetlenie komunikatu sukcesu i odświeżenie listy fiszek.
- **Aktualizacja fiszki:**  
  - Metoda: PUT `/cards/{id}`  
  - Payload: `{ front, back }`  
  - Po udanej odpowiedzi – aktualizacja danych w interfejsie i wyświetlenie komunikatu sukcesu.
  
Obsługa komunikacji z API odbywa się przez fetch lub bibliotekę HTTP (np. axios), przy użyciu asynchronicznych wywołań i odpowiedniej obsługi błędów (try-catch).

## 8. Interakcje użytkownika
- Kliknięcie przycisku otwierającego modal z dashboardu.  
- Wprowadzenie danych w polach formularza z natychmiastową walidacją (inline).  
- Kliknięcie przycisku "Zapisz" oraz wywołanie API – komunikaty sukcesu lub błędu wyświetlane jako toast notifications.  
- W przypadku edycji – pola pre-populowane danymi fiszki, a zmiany zatwierdzane po kliknięciu przycisku "Zapisz zmiany".

## 9. Warunki i walidacja
- Walidacja wykonywana lokalnie przed wysłaniem żądania do API:
  - Sprawdzanie minimalnej i maksymalnej długości pól („front”: 1–1000 znaków, „back”: 1–5000 znaków).
  - Weryfikacja, czy pola nie są puste.
- Po stronie API dodatkowa walidacja oraz obsługa komunikatów błędów:
  - W przypadku błędów wyświetlenie toast notification z komunikatem zwrotnym z serwera.

## 10. Obsługa błędów
- **Błędy walidacji lokalnej:**  
  Natychmiastowe wyświetlanie komunikatów przy nieprawidłowych danych.
- **Błędy z API:**  
  Obsługa błędów poprzez catch, wyświetlenie toast notification oraz ewentualne podświetlenie pól formularza.
- **Scenariusze brzegowe:**  
  - Utrata połączenia z API – wyświetlenie komunikatu „Błąd połączenia”  
  - Błędny token lub autoryzacja – przekierowanie użytkownika do logowania

## 11. Kroki implementacji
1. Utworzenie szkieletu modali (CreateFlashcardModal i EditFlashcardModal) jako osobnych komponentów wewnątrz folderu komponentów dashboardu.
2. Zaimplementowanie pól formularza oraz przycisków, wraz z podstawową strukturą HTML/CSS (z użyciem TailwindCSS i komponentów MudBlazor, jeśli dostępne).
3. Dodanie logiki lokalnego stanu za pomocą useState oraz ewentualnego custom hooka `useFlashcardForm` do obsługi zmian wartości pól.
4. Implementacja walidacji inline zgodnie z wyznaczonymi warunkami (minimalna i maksymalna długość pól).
5. Stworzenie funkcji obsługujących wysyłanie żądania do API (POST dla tworzenia, PUT dla edycji) z obsługą błędów.
6. Dodanie mechanizmu wyświetlania toast notifications – zarówno komunikatów sukcesu, jak i błędów.
7. Integracja obu modali z widokiem dashboardu poprzez przekazywanie odpowiednich propsów (isOpen, onClose, initialData/originalContentId).
8. Testowanie komponentów – walidacja danych, interakcje użytkownika, i symulacja odpowiedzi z API.
9. Przegląd kodu, docelowe testy manualne i ewentualne poprawki w UX dla dostępności i intuicyjności interfejsu.