using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Bictionary.Data;
using Bictionary.Models;

namespace Bictionary;

public partial class DictionaryEditorWindow : Window
{
    private readonly WordRepo wordRepo = new();

    public DictionaryEditorWindow()
    {
        InitializeComponent();

        Opened += DictionaryEditorWindow_Opened;
    }

    private async void DictionaryEditorWindow_Opened(object? sender, EventArgs e)
    {
        await RefreshProgressAsync();

        WordInput.Focus();
    }

    private async void SaveWordButton_Click(object? sender, EventArgs e)
    {
        string wordText = WordInput.Text?.Trim() ?? "";
        string partOfSpeech = PartOfSpeechInput.Text?.Trim() ?? "";
        string definition = DefinitionInput.Text?.Trim() ?? "";
        string? example = GetOptionalText(ExampleInput.Text);

        if (!IsEntryValid(
            wordText,
            partOfSpeech,
            definition
        ))
        {
            return;
        }

        Word word = new()
        {
            Text = wordText,
            PartOfSpeech = partOfSpeech,
            Definition = definition,
            Example = example
        };

        try
        {
            SaveWordButton.IsEnabled = false;

            Word savedWord = await wordRepo.AddWordAsync(word);

            EditorStatusTextBlock.Text = $"'{savedWord.Text}' was saved.";

            ClearEntryForm();

            await RefreshProgressAsync();

            WordInput.Focus();
        }
        catch (Exception exception)
        {
            EditorStatusTextBlock.Text =
                $"Unable to save the word: {exception.Message}";
        }
        finally
        {
            SaveWordButton.IsEnabled = true;
        }
    }

    private bool IsEntryValid(
        string wordText,
        string partOfSpeech,
        string definition
    )
    {
        if (string.IsNullOrWhiteSpace(wordText))
        {
            EditorStatusTextBlock.Text = "Please enter a word.";

            WordInput.Focus();

            return false;
        }

        if (string.IsNullOrWhiteSpace(partOfSpeech))
        {
            EditorStatusTextBlock.Text = "Please enter a part of speech.";

            PartOfSpeechInput.Focus();

            return false;
        }

        if (string.IsNullOrWhiteSpace(definition))
        {
            EditorStatusTextBlock.Text = "Please enter a definition.";

            DefinitionInput.Focus();

            return false;
        }

        return true;
    }

    private static string? GetOptionalText(string? text)
    {
        string? trimmedText = text?.Trim();

        return string.IsNullOrWhiteSpace(trimmedText)
            ? null
            : trimmedText;
    }

    private void ClearEntryForm()
    {
        WordInput.Text ="";
        PartOfSpeechInput.Text = "";
        DefinitionInput.Text = "";
        ExampleInput.Text = "";
    }

    private async Task RefreshProgressAsync()
    {
        int wordCount =
            await wordRepo.GetWordCountAsync();

        List<Word> recentWords =
            await wordRepo.GetRecentWordsAsync();

        WordCountTextBlock.Text =
            $"Total words: {wordCount}";

            if (recentWords.Count == 0)
        {
            LastWordTextBlock.Text = 
                "Last word added: -";

            RecentWordsListBox.ItemsSource =
                Array.Empty<string>();

            return;
        }

        LastWordTextBlock.Text =
            $"Last word added: {recentWords[0].Text}";
        
        List<string> recentWordNames = [];

        foreach (Word word in recentWords)
        {
            recentWordNames.Add(
                $"{word.Id}. {word.Text} - {word.PartOfSpeech}"
            );
        }

        RecentWordsListBox.ItemsSource = recentWordNames;
    }
}