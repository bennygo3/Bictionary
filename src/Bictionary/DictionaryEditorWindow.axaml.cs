using Avalonia.Controls;
using Bictionary.Data;

namespace Bictionary;

public partial class DictionaryEditorWindow : Window
{
    private readonly WordRepo wordRepo = new();

    public DictionaryEditorWindow()
    {
        InitializeComponent();
    }
}