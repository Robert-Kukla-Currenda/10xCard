-- 1. Tabela: Users
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- 2. Tabela: Flashcards
CREATE TABLE flashcards (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    content TEXT NOT NULL,
    summary TEXT NOT NULL,
    source VARCHAR(50) NOT NULL, -- dozwolone wartości: 'ai', 'user'
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_flashcards_user
      FOREIGN KEY (user_id)
      REFERENCES users(id)
      ON DELETE CASCADE
);

-- 3. Tabela: Logs
CREATE TABLE logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    flashcard_id UUID,
    operation VARCHAR(10) NOT NULL, -- dozwolone wartości: 'INSERT', 'UPDATE', 'DELETE'
    performed_at TIMESTAMP NOT NULL DEFAULT NOW(),
    details JSONB,
    CONSTRAINT fk_logs_user
      FOREIGN KEY (user_id)
      REFERENCES users(id)
      ON DELETE CASCADE,
    CONSTRAINT fk_logs_flashcard
      FOREIGN KEY (flashcard_id)
      REFERENCES flashcards(id)
      ON DELETE SET NULL
);

-- 4. Indeksy
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_flashcards_user_id ON flashcards(user_id);
CREATE INDEX idx_logs_user_id ON logs(user_id);
CREATE INDEX idx_logs_flashcard_id ON logs(flashcard_id);

-- 5. Zasady PostgreSQL (RLS)
-- Włącz RLS dla tabeli flashcards
ALTER TABLE flashcards ENABLE ROW LEVEL SECURITY;
CREATE POLICY user_flashcards_policy ON flashcards
    USING (user_id = current_setting('app.current_user_id')::UUID);

-- Włącz RLS dla tabeli logs
ALTER TABLE logs ENABLE ROW LEVEL SECURITY;
CREATE POLICY user_logs_policy ON logs
    USING (user_id = current_setting('app.current_user_id')::UUID);

-- 6. Widoki
CREATE VIEW user_flashcards AS
    SELECT id, user_id, content, summary, source, created_at, updated_at
    FROM flashcards;

-- Dodatkowe uwagi:
-- a) Wykorzystywane są funkcje gen_random_uuid() oraz current_setting() - należy upewnić się, 
--    że rozszerzenie pgcrypto jest zainstalowane oraz zmienna 'app.current_user_id' ustawiana jest 
--    przy każdym połączeniu użytkownika.
-- b) Schemat bazy danych jest zaprojektowany zgodnie z wymaganiami MVP, z relacją 1:N między tabelami users i flashcards,
--    logowaniem operacji w tabeli logs oraz wdrożeniem RLS dla ochrony danych użytkowników.