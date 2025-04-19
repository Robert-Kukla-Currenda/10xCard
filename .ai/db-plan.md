# Schemat bazy danych dla 10xCards

## 1. Tabele i kolumny

### Tabela: users
- id: SERIAL PRIMARY KEY  
- email: VARCHAR(255) NOT NULL UNIQUE  
- first_name: VARCHAR(100) NOT NULL  
- last_name: VARCHAR(100) NOT NULL  
- password_hash: VARCHAR(255) NOT NULL  
- created_at: TIMESTAMPTZ NOT NULL DEFAULT now()

### Tabela: cards
- id: SERIAL PRIMARY KEY  
- user_id: INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE  
- original_content: TEXT NOT NULL CHECK (char_length(original_content) BETWEEN 1000 AND 10000)  
- front: TEXT NOT NULL CHECK (char_length(front) BETWEEN 1 AND 1000)  
- back: TEXT NOT NULL CHECK (char_length(back) BETWEEN 1 AND 5000)  
- generated_by: VARCHAR(10) NOT NULL CHECK (created_by IN ('AI', 'human'))  
- created_at: TIMESTAMPTZ NOT NULL DEFAULT now()

### Tabela: error_logs
- id: SERIAL PRIMARY KEY  
- card_id: INTEGER NOT NULL REFERENCES cards(id) ON DELETE CASCADE  
- error_details: TEXT NOT NULL  
- logged_at: TIMESTAMPTZ NOT NULL DEFAULT now()

## 2. Relacje między tabelami
- **users** → **cards**: relacja jeden-do-wielu (każdy użytkownik może mieć wiele fiszek).  
- **cards** → **error_logs**: relacja jeden-do-wielu (każda fiszka może mieć wiele logów błędów).

## 3. Indeksy
- Indeks na kolumnie `users.email` (unikalny).  
- Indeks na kolumnie `users(first_name, last_name)` dla optymalizacji wyszukiwania po nazwie użytkownika.  
- Indeks na kolumnie `cards.user_id` dla szybkiego filtrowania fiszek według użytkownika.  
- Opcjonalnie: pełnotekstowy indeks (np. GIN) na kolumnach `cards.original_content`, `cards.front` i `cards.back` w celu optymalizacji wyszukiwania słów kluczowych.

## 4. Zasady PostgreSQL - Row Level Security (RLS)
```sql
-- Włączenie RLS dla tabeli cards
ALTER TABLE cards ENABLE ROW LEVEL SECURITY;

-- Przykładowa polityka RLS umożliwiająca użytkownikowi dostęp tylko do własnych fiszek.
CREATE POLICY user_cards_policy ON cards
  USING (user_id = current_setting('app.current_user_id')::INTEGER)
  WITH CHECK (user_id = current_setting('app.current_user_id')::INTEGER);
```

## 5. Dodatkowe uwagi
- Wszystkie pola tekstowe w tabeli cards mają nałożone ograniczenia CHECK, aby zapewnić zgodność z wymaganiami dotyczącymi długości treści (1000–10000 znaków dla original_content oraz minimalna długość dla front i back).
- Wykorzystanie typu TIMESTAMPTZ umożliwia prawidłowe zarządzanie strefami czasowymi przy datowaniach (np. created_at i logged_at).
- Do wyszukiwania fiszek po słowach kluczowych można rozważyć wykorzystanie pełnotekstowego wyszukiwania z indeksem GIN.
- Implementacja RLS opiera się na ustawieniu zmiennej konfiguracyjnej app.current_user_id, która musi być odpowiednio zarządzana w kontekście uwierzytelnionego użytkownika.