CREATE TABLE IF NOT EXISTS warn_username_history (
    id         INTEGER  PRIMARY KEY AUTOINCREMENT,
    from_id    INTEGER,
    first_name VARCHAR,
    last_name  VARCHAR,
    step_count INTEGER,
    chat_id    INTEGER,
    created_at DATETIME,
    updated_at DATETIME
);
