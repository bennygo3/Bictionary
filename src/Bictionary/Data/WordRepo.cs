// using System;
// using System.Collections.Generic;
// using System.Linq;
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
                ? ""
                : reader.GetString(4)
        };
    }
}