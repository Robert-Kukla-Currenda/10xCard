# Plan implementacji widoku generate

## 1. Przegląd
Widok ten pozwala użytkownikowi wprowadzić tekst źródłowy (1000–10000 znaków) i skorzystać z usługi AI do wygenerowania fiszek. Użytkownik może następnie edytować wygenerowany front i tył fiszek oraz zapisać je w systemie.

## 2. Routing widoku
Widok będzie dostępny pod ścieżką:  
`/cards/generate`

## 3. Struktura komponentów
1. **GenerateCardPage** (strona główna widoku)  
   - Zawiera formularz do wpisania treści oraz sekcję z wygenerowanymi fiszkami.  
2. **GenerateForm** (formularz generowania)  
   - Pole tekstowe do wprowadzenia treści źródłowej oraz przycisk "Generuj".  
3. **GeneratedCardPreview** (podgląd wygenerowanych fiszek)  
   - Wyświetla pola front i back wraz z możliwością edycji.  
   - Zawiera przycisk zapisu, który wysyła wszystkie wygenerowane fiszki do API.

## 4. Szczegóły komponentów

### GenerateCardPage
- **Opis**: Komponent rodzic kontrolujący przepływ danych pomiędzy formularzem generowania a miejscem wyświetlania rezultatów.
- **Główne elementy**:
  - Sekcja formularza (komponent **GenerateForm**).
  - Sekcja podglądu fiszek (komponent **GeneratedCardPreview**).
- **Obsługiwane interakcje**:
  - Otrzymanie danych wygenerowanych przez AI z API.
  - Przekazanie danych do podglądu fiszek.
- **Walidacja**: Sprawdzenie minimalnej i maksymalnej długości tekstu (1000–10000 znaków) przed wysłaniem do API.
- **Typy**:  
  - Zależne od DTO: `GenerateCardCommand`, `CardDto`.  
- **Propsy**:  
  - Brak bezpośrednich, komponuje dzieci.

### GenerateForm
- **Opis**: Formularz wpisywania treści źródłowej i inicjacji żądania generowania fiszek.
- **Główne elementy**:
  - Pole wielowierszowe do wprowadzenia tekstu.
  - Przycisk "Generuj".
- **Obsługiwane interakcje**:
  - OnSubmit – walidacja długości tekstu.
  - Kliknięcie "Generuj" – wysłanie żądania POST do `/api/cards/generate`.
- **Walidacja**:
  - Tekst min. 1000 i max. 10000 znaków.
- **Typy**:
  - `GenerateCardCommand` jako model żądania.
- **Propsy**:  
  - Funktor lub handler typu `(originalContent: string) => void`, wywoływany przy prawidłowym wprowadzeniu danych.

### GeneratedCardPreview
- **Opis**: Wyświetla pola front i back wygenerowanych fiszek, pozwala na ewentualną korektę i zapis wszystkich fiszek.
- **Główne elementy**:
  - Dwa pola tekstowe: "Front" i "Back".
  - Przycisk "Zapisz".
- **Obsługiwane interakcje**:
  - Edycja pól front/back.
  - Kliknięcie "Zapisz" – wysłanie danych nowej fiszki do osobnego żądania POST/PUT (według logiki w aplikacji) lub wykorzystanie istniejącej komendy `SaveCardCommand`.
- **Walidacja**:
  - Front min. 1, max. 1000 znaków; Back min. 1, max. 5000 znaków.
- **Typy**:
  - `CardDto` lub nowy widokowy model z polami `front`, `back`.
- **Propsy**:
  - `cardData: CardDto` (lub tymczasowy model)
  - Handler zapisu `onSave: (cardData: Partial<CardDto>) => void`

## 5. Typy
- **GenerateCardCommand**  
  - `originalContent` (string, 1000–10000 znaków).  
- **CardDto**  
  - `id`, `userId`, `originalContent`, `front`, `back`, `generatedBy`, `createdAt`, `updatedAt?`.
- **SaveCardCommand** (o ile potrzebny przy zapisie)  
  - `front` (1–1000 znaków), `back` (1–5000 znaków), `generatedBy`, `originalContent` (opcjonalnie).

## 6. Zarządzanie stanem
- **Stan lokalny** w komponencie rodzica **GenerateCardPage** przechowuje:
  - Wartość tekstu źródłowego wprowadzonego przez użytkownika.
  - Otrzymany obiekt fiszki (`CardDto`) z API.
- **Customowy hook** potencjalnie do obsługi logiki walidacji lub wywołań API, jeśli rozdzielenie logiki okaże się potrzebne.

## 7. Integracja API
- **Generowanie fiszki**  
  - `POST /cards/generate` z `GenerateCardCommand`.  
  - Odpowiedź: `CardDto` z wypełnionymi polami `front`, `back`, `originalContent`.  
- **Zapis fiszki**  
  - Opcjonalnie `POST /cards` lub inny punkt końcowy, jeśli istnieje w aplikacji.

## 8. Interakcje użytkownika
1. Użytkownik wprowadza tekst w polu (min. 1000, max. 10000 znaków).  
2. Naciska "Generuj" → żądanie do `/cards/generate`.  
3. Otrzymane dane że AI pojawiają się w polach front/back.  
4. Użytkownik może je zmodyfikować i nacisnąć "Zapisz".  

## 9. Warunki i walidacja
- **Wprowadzenie tekstu**: 1000–10000 znaków (walidacja w `GenerateForm`).  
- **Front**: 1–1000 znaków, **Back**: 1–5000 znaków.  
- Wyświetlanie komunikatów o błędach (np. toast) przy naruszeniu warunków.

## 10. Obsługa błędów
- 400 (długość tekstu nieprawidłowa) → komunikat walidacyjny.  
- 401 (brak autoryzacji) → przekierowanie do logowania.  
- 422 (problem generowania) → toast z informacją o błędzie.  
- Inne błędy → komunikat ogólny o niepowodzeniu.

## 11. Kroki implementacji
1. Stworzenie nowej strony **GenerateCardPage** i konfiguracja routingu `/cards/generate`.  
2. Zaimplementowanie formularza generowania (komponent **GenerateForm**) z walidacją.  
3. Obsługa wywołania API `/cards/generate` w metodzie onSubmit.  
4. Przekazanie wyniku do **GeneratedCardPreview** i umożliwienie edycji.  
5. Implementacja logiki zapisu – wysłanie nowej fiszki do odpowiedniego endpointu.  
6. Dodanie obsługi błędów i komunikatów toast.  
7. Finalne testy funkcjonalne i integracyjne.