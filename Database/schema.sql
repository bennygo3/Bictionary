CREATE TABLE words (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    text VARCHAR(100) NOT NULL UNIQUE,
    part_of_speech VARCHAR(20) NOT NULL,
    definition TEXT NOT NULL,
    example TEXT
);

CREATE UNIQUE INDEX words_text_lower_unique
ON words (LOWER(text));