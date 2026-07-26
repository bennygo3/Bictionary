using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bictionary.Models;
using Npgsql;

namespace Bictionary.Data;

public class WordRepo
{
    private readonly NpgsqlDataSource dataSource;

    public WordRepo()
        {
        string connectionString =
            "Host=localhost;Username=benj.a.gomez;Database=bictionary";

        dataSource = NpgsqlDataSource.Create(connectionString);
    }

    public async Task<Word?> FindWordAsync(string searchText)
    {
        const string sql = """ 
            SELECT 
                id,
                text,
                part_of_speech,
                definition,
                example
            FROM words
            WHERE LOWER(text) = LOWER(@searchText)
            LIMIT 1;
            """;
        
        await using NpgsqlCommand command =
            dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("searchText", searchText);

        await using NpgsqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Word
        {
            Id = reader.GetInt32(0),
            Text = reader.GetString(1),
            PartOfSpeech = reader.GetString(2),
            Definition = reader.GetString(3),
            Example = reader.IsDBNull(4)
                ? null
                : reader.GetString(4)
        };
    }

    public async Task<Word> AddWordAsync(Word word)
    {
        const string sql ="""
            INSERT INTO words (
                text,
                part_of_speech,
                definition,
                example
            )
            VALUES (
                @text,
                @partOfSpeech,
                @definition,
                @example
            )
            RETURNING id;
            """;
        
        await using NpgsqlCommand command = 
            dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("text", word.Text);
        command.Parameters.AddWithValue("partOfSpeech", word.PartOfSpeech);
        command.Parameters.AddWithValue("definition", word.Definition);
        command.Parameters.AddWithValue("example", NpgsqlTypes.NpgsqlDbType.Text, (object?)word.Example ?? DBNull.Value);

        object? result = await command.ExecuteScalarAsync();

        if (result is not int insertedId)
        {
            throw new InvalidOperationException(
                "The word was not added."
            );
        }

        word.Id = insertedId;

        return word;
    }

    public async Task<List<Word>> GetRecentWordsAsync(
        int limit = 10
    )
    {
        const string sql = """
            SELECT 
                id,
                text,
                part_of_speech,
                definition,
                example
            FROM words
            ORDER BY id DESC
            LIMIT @limit;
            """;

        await using NpgsqlCommand command =
            dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("limit", limit);

        await using NpgsqlDataReader reader = 
            await command.ExecuteReaderAsync();
        
        List<Word> recentWords = [];

        while (await reader.ReadAsync())
        {
            Word word = new()
            {
                Id = reader.GetInt32(0),
                Text = reader.GetString(1),
                PartOfSpeech = reader.GetString(2),
                Definition = reader.GetString(3),
                Example = reader.IsDBNull(4)
                    ? null
                    : reader.GetString(4)
            };

            recentWords.Add(word);
        }

        return recentWords;
    }

    public async Task<int> GetWordCountAsync()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM words;
            """;
        
        await using NpgsqlCommand command = 
            dataSource.CreateCommand(sql);

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
}