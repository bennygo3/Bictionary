using System;
using System.Collections.Generic;
using System.Linq;
using Bictionary.Models;

namespace Bictionary.Data;

public class WordRepo
{
    private readonly List<Word> words =
    [
        new()
        {
            Text = "apple",
            PartOfSpeech = "noun",
            Definition = "A round fruit with red, green, or yellow skin.",
            Example = "I ate an apple with lunch."
        },

        new()
        {
            Text = "algorithm",
            PartOfSpeech = "noun",
            Definition = "A sequence of instructions used to solve a problem",
            Example = "Binary search is an efficient algorithm."
        },

        new()
        {
            Text = "computer",
            PartOfSpeech = "noun",
            Definition = "An electronic device that processes data.",
            Example = "I use my computer every day."
        },

        new()
        {
            Text = "dictionary",
            PartOfSpeech = "noun",
            Definition = "A reference source containing words and their meanings.",
            Example = "She looked up the word in the dictionary."
        },

        new()
        {
            Text = "dog",
            PartOfSpeech = "noun",
            Definition = "A domesticated mammal commonly kept as a pet.",
            Example = "The dog wagged its tail."
        }

    ];

    public Word? FindWord(string searchText)
    {
        return words.FirstOrDefault(word =>
            string.Equals(
                word.Text,
                searchText,
                StringComparison.OrdinalIgnoreCase
            )
        );
    }
}