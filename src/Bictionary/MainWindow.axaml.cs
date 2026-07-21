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
        DefinitionTextBlock.Text = MainSearchBox.Text;

        if (MainSearchBox.Text.Length == 0)
        {
            DefinitionTextBlock.Text = "Please enter a word to search";
        } else
        {
            DefinitionTextBlock.Text = MainSearchBox.Text;
        }
        
    }
}