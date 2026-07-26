using System.Threading.Tasks;
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

    private async void MainSearchButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await SearchForWordAsync();
    }

    private void OpenEditorButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DictionaryEditorWindow editorWindow = new();

        editorWindow.Show();
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

        ExampleTextBlock.Text = word.Example ?? "-";
    }

    private void DisplayWordNotFound(string searchText)
    {
        SearchStatusTextBlock.Text = "No definition found.";

        WordTextBlock.Text = searchText;
        PartOfSpeechTextBlock.Text = "-";
        DefinitionTextBlock.Text = "-";
        ExampleTextBlock.Text = "-";
    }

    private async void MainSearchBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter)
        {
           await SearchForWordAsync();
        }
    }

    // private void SearchForWord()
    private async Task SearchForWordAsync()
    {
        string searchText = MainSearchBox.Text?.Trim() ?? "";

        if (!IsSearchInputValid(searchText))
            return;

        Word? matchingWord = await wordRepo.FindWordAsync(searchText);

        if (matchingWord is null)
        {
            DisplayWordNotFound(searchText);
            return;
        }

        DisplayWord(matchingWord);
    }

}