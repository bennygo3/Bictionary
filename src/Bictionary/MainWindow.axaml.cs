using Avalonia.Controls;
using Bictionary.Data;
using Bictionary.Models;

namespace Bictionary;

public partial class MainWindow : Window
{

    private readonly WordRepo wordRepo = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainSearchButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SearchForWord();
    }

    private bool IsSearchInputValid(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            SearchStatusTextBlock.Text = "Please enter a word to search.";

            return false;
        }

        return true;
    }

    private void DisplayWord(Word word)
    {
        SearchStatusTextBlock.Text = "";

        WordTextBlock.Text = word.Text;

        PartOfSpeechTextBlock.Text = word.PartOfSpeech;

        DefinitionTextBlock.Text = word.Definition;

        ExampleTextBlock.Text = word.Example;
    }

    private void DisplayWordNotFound(string searchText)
    {
        SearchStatusTextBlock.Text = "No definition found.";

        WordTextBlock.Text = searchText;
        PartOfSpeechTextBlock.Text = "-";
        DefinitionTextBlock.Text = "-";
        ExampleTextBlock.Text = "-";
    }

    private void MainSearchBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter)
        {
            SearchForWord();
        }
    }

    private void SearchForWord()
    {
        string searchText = MainSearchBox.Text?.Trim() ?? "";

        if (!IsSearchInputValid(searchText))
            return;

        Word? matchingWord = wordRepo.FindWord(searchText);

        if (matchingWord is null)
        {
            DisplayWordNotFound(searchText);
            return;
        }

        DisplayWord(matchingWord);
    }

}