using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Bictionary.Data;
using Bictionary.Models;
using Npgsql;

namespace Bictionary;

public partial class DictionaryEditorWindow : Window
{
    private readonly WordRepo wordRepo = new();

    private List<Word> recentWords = [];

    private Word? wordBeingEdited;

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

    private async void SaveWordButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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
            Id = wordBeingEdited?.Id ?? 0,
            Text = wordText,
            PartOfSpeech = partOfSpeech,
            Definition = definition,
            Example = example
        };

        try
        {
            SaveWordButton.IsEnabled = false;

            Word savedWord;

            if (wordBeingEdited is null)
            {
                savedWord = await wordRepo.AddWordAsync(word);

                EditorStatusTextBlock.Text = $"'{savedWord.Text}' was saved.";
            }
            else
            {
                savedWord = await wordRepo.UpdateWordAsync(word);

                EditorStatusTextBlock.Text = $"'{savedWord.Text}' was updated.";
            }

            ClearEntryForm();

            EndEditing();

            await RefreshProgressAsync();

            WordInput.Focus();
        }
        catch (PostgresException exception)
            when (exception.SqlState == "23505")
        {
            EditorStatusTextBlock.Text = "An identical entry already exists.";
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
        int wordCount = await wordRepo.GetWordCountAsync();

        recentWords = await wordRepo.GetRecentWordsAsync();

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

        LastWordTextBlock.Text = $"Last word added: {recentWords[0].Text}";
        
        List<string> recentWordNames = [];

        foreach (Word word in recentWords)
        {
            recentWordNames.Add(
                $"{word.Id}. {word.Text} - {word.PartOfSpeech}"
            );
        }

        RecentWordsListBox.ItemsSource = recentWordNames;
    }

    private void RecentWordsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        int selectedIndex = RecentWordsListBox.SelectedIndex;

        if (selectedIndex < 0 || selectedIndex >= recentWords.Count)
        {
            return;
        }

        Word selectedWord = recentWords[selectedIndex];

        BeginEditingWord(selectedWord);
    }

    private void BeginEditingWord(Word word)
    {
        wordBeingEdited = word;

        WordInput.Text = word.Text;
        PartOfSpeechInput.Text = word.PartOfSpeech;
        DefinitionInput.Text = word.Definition;
        ExampleInput.Text = word.Example ?? "";

        SaveWordButton.Content = "Update Entry";

        CancelEditButton.IsVisible = true;

        EditorStatusTextBlock.Text = $"Editing entry #{word.Id}: {word.Text}";
    }

    private void CancelEditButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClearEntryForm();

        EndEditing();

        EditorStatusTextBlock.Text = "Editing canceled.";

        WordInput.Focus();
    }

    private void EndEditing()
    {
        wordBeingEdited = null;

        SaveWordButton.Content = "Save Word";

        CancelEditButton.IsVisible = false;

        RecentWordsListBox.SelectedIndex = -1;
    }
}