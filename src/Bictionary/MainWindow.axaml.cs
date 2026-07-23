using Avalonia.Controls;
using Bictionary.Models;

namespace Bictionary;

public partial class MainWindow : Window
{
    private readonly Word apple = new()
    {
        Text = "apple",
        PartOfSpeech = "noun",
        Definition = "A round fruit with red, green, or yellow skin.",
        Example = "I ate an apple with lunch."
    };
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainSearchButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!IsSearchInputValid(MainSearchBox.Text))
            return;

        DisplayWord(apple);
        // SearchStatusTextBlock.Text = "";
        // WordTextBlock.Text = MainSearchBox.Text;

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
}