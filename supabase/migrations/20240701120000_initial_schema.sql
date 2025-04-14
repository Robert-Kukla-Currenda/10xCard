-- migration: initial schema creation for flashcards application
-- affected tables: users, flashcards, logs
-- description: sets up core tables with proper relationships and security policies

-- enable pgcrypto extension for uuid generation
create extension if not exists "pgcrypto";

-- 1. users table
create table users (
    id uuid primary key default gen_random_uuid(),
    email varchar(255) not null unique,
    first_name varchar(100) not null,
    last_name varchar(100) not null,
    password_hash text not null,
    created_at timestamp not null default now()
);

-- enable row level security on users table
alter table users enable row level security;

-- create policy for users to read only their own data
create policy "users can read their own data" on users
    for select using (auth.uid() = id);

-- create policy for users to update their own data
create policy "users can update their own data" on users
    for update using (auth.uid() = id);

-- 2. flashcards table
create table flashcards (
    id uuid primary key default gen_random_uuid(),
    user_id uuid not null,
    content text not null,
    summary text not null,
    source varchar(50) not null, -- allowed values: 'ai', 'user'
    created_at timestamp not null default now(),
    updated_at timestamp not null default now(),
    constraint fk_flashcards_user
      foreign key (user_id)
      references users(id)
      on delete cascade
);

-- enable row level security on flashcards table
alter table flashcards enable row level security;

-- create policy for users to read their own flashcards
create policy "users can read their own flashcards" on flashcards
    for select using (auth.uid() = user_id);

-- create policy for users to insert their own flashcards
create policy "users can insert their own flashcards" on flashcards
    for insert with check (auth.uid() = user_id);

-- create policy for users to update their own flashcards
create policy "users can update their own flashcards" on flashcards
    for update using (auth.uid() = user_id);

-- create policy for users to delete their own flashcards
create policy "users can delete their own flashcards" on flashcards
    for delete using (auth.uid() = user_id);

-- 3. logs table for tracking operations
create table logs (
    id uuid primary key default gen_random_uuid(),
    user_id uuid not null,
    flashcard_id uuid,
    operation varchar(10) not null, -- allowed values: 'INSERT', 'UPDATE', 'DELETE'
    performed_at timestamp not null default now(),
    details jsonb,
    constraint fk_logs_user
      foreign key (user_id)
      references users(id)
      on delete cascade,
    constraint fk_logs_flashcard
      foreign key (flashcard_id)
      references flashcards(id)
      on delete set null
);

-- enable row level security on logs table
alter table logs enable row level security;

-- create policy for users to read their own logs
create policy "users can read their own logs" on logs
    for select using (auth.uid() = user_id);

-- create policy for authenticated users to insert logs
create policy "authenticated users can insert logs" on logs
    for insert with check (auth.role() = 'authenticated' and auth.uid() = user_id);

-- 4. indexes for optimization
create index idx_users_email on users(email);
create index idx_flashcards_user_id on flashcards(user_id);
create index idx_logs_user_id on logs(user_id);
create index idx_logs_flashcard_id on logs(flashcard_id);

-- 5. useful views
create view user_flashcards as
    select id, user_id, content, summary, source, created_at, updated_at
    from flashcards;

-- enable row level security on views
alter view user_flashcards enable row level security;

-- create policy for the view
create policy "users can view their flashcards" on user_flashcards
    for select using (auth.uid() = user_id);
