using Avalonia.Controls;

namespace Bictionary;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainSearchButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!IsSearchValidInput())
            return;

        DefinitionTextBlock.Text = MainSearchBox.Text;

    }

    private bool IsSearchValidInput()
    {
        if (string.IsNullOrWhiteSpace(MainSearchBox.Text))
        {
            DefinitionTextBlock.Text = "Please enter a word to search.";

            return false;
        }

        return true;
    }
}