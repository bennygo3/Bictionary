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
    }
}