# Dokument wymagań produktu (PRD) - 10xCards

## 1. Przegląd produktu
Aplikacja web mająca na celu automatyczne generowanie wysokiej jakości fiszek edukacyjnych przy użyciu sztucznej inteligencji. System zostanie zaprojektowany tak, aby oszczędzić użytkownikom czas potrzebny na manualne tworzenie fiszek, wspierając metodę spaced repetition. Produkt oferuje zarówno możliwość automatycznego generowania fiszek, jak i ręczną edycję oraz tworzenie treści.

## 2. Problem użytkownika
Manualne tworzenie fiszek jest czasochłonne, co zniechęca użytkowników do korzystania z metody spaced repetition. Brak narzędzia wspomagającego generowanie fiszek skutkuje opóźnieniami w procesie nauki, a użytkownicy nie znajdują czasu na regularne aktualizacje swojego materiału edukacyjnego.

## 3. Wymagania funkcjonalne
1. Generowanie fiszek przez AI
   - System przyjmuje tekst o długości od 1000 do 10000 znaków.
   - Użycie technik streszczenia i redakcji treści do wygenerowania zestawu fiszek.
   - Użytkownik ma możliwość edycji treści wygenerowanej przez AI przed zatwierdzeniem.
   - Tekst na frontowej strona pojedynczej fiszki nie może być dłuższy niż 1000 znaków.
   - Tekst pojedynczej fiszki na tylnej stronie, nie może być dłuższy niż 5000 znaków.
   
2. Manualne tworzenie fiszek
   - Umożliwienie użytkownikowi tworzenia, edycji, przeglądania i usuwania fiszek ręcznie.
   - Możliwe jest manualne dodanie fiszki do fiszek wygenerowanych przez AI
   
3. System kont użytkowników
   - Prosta rejestracja i logowanie.
   - Bezpieczne przechowywanie oraz zarządzanie fiszkami z poziomu konta użytkownika.
   
4. Integracja z prostym algorytmem powtórek
   - Fiszki będą integrowane z gotowym, prostym algorytmem powtórek, bez wprowadzania własnych zaawansowanych rozwiązań.

## 4. Granice produktu
1. Elementy niewchodzące w zakres MVP:
   - Własny, zaawansowany algorytm powtórek (np. SuperMemo, Anki).
   - Import plików w wielu formatach (PDF, DOCX, itp.).
   - Współdzielenie fiszek między użytkownikami.
   - Integracje z innymi platformami edukacyjnymi.
   - Rozwój aplikacji mobilnej; produkt ma początkowo funkcjonować wyłącznie jako aplikacja web.
   
2. Wymagania jakościowe i ograniczenia:
   - Interfejs musi być łatwy w obsłudze przez osoby słabowidzące (należy określić szczegółowe wytyczne dotyczące kontrastu, rozmiaru czcionki itp.).
   - Responsywność nie jest priorytetem.
   - Treść wejściowa do fiszki musi zawierać od 1000 do 10000 znaków.

## 5. Historyjki użytkowników
US-001  
- Tytuł: Generowanie fiszek przez AI  
- Opis: Jako użytkownik chcę wkleić tekst o długości 1000-10000 znaków, aby AI wygenerowało zestaw fiszek wykorzystując techniki streszczenia i redakcji treści, co pozwoli mi zaoszczędzić czas.  
- Kryteria akceptacji:
  1. Użytkownik wkleja tekst mieszczący się w określonym przedziale znaków.
  2. System generuje fiszki na podstawie wprowadzonego tekstu.
  3. Użytkownik może edytować wygenerowaną treść przed jej zatwierdzeniem.

US-002  
- Tytuł: Manualne tworzenie fiszek  
- Opis: Jako użytkownik chcę móc ręcznie tworzyć fiszek, aby mieć pełną kontrolę nad treścią.  
- Kryteria akceptacji:
  1. Użytkownik otrzymuje interfejs do wprowadzania treści fiszek.
  2. Fiszki są zapisywane i dostępne w systemie po zatwierdzeniu.

US-003  
- Tytuł: Przeglądanie, edycja i usuwanie fiszek  
- Opis: Jako użytkownik chcę przeglądać, edytować i usuwać istniejące fiszki, aby móc zarządzać swoimi materiałami edukacyjnymi.  
- Kryteria akceptacji:
  1. Użytkownik widzi listę treści oraz powiązanych z nią fiszek.
  2. Fiszki są zagnieżdżone pod treścią, z której zostały utworzone.
  3. Użytkownik może wybrać fiszkę do edycji lub usunięcia.
  4. Zmiany są natychmiast zapisywane i widoczne w systemie.

US-004  
- Tytuł: Rejestracja i logowanie użytkownika  
- Opis: Jako użytkownik chcę móc zarejestrować się i zalogować do systemu, aby uzyskać dostęp do moich fiszek.  
- Kryteria akceptacji:
  1. Użytkownik może utworzyć konto przy użyciu adresu email i hasła.
  2. Użytkownik może bez problemu logować się i wylogowywać.
  3. Dane logowania są przechowywane w sposób bezpieczny.

## 6. Metryki sukcesu
1. Akceptacja fiszek generowanych przez AI
   - Minimum 75% fiszek wygenerowanych przez AI musi być zaakceptowanych przez użytkowników.
   
2. Wykorzystanie funkcji AI
   - Co najmniej 75% fiszek stworzonych w systemie ma być generowanych przy użyciu AI.
   
3. Czas generowania
   - Czas potrzebny na generowanie fiszek przez AI nie powinien znacząco opóźniać całego procesu tworzenia.
