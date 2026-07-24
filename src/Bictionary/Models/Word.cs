namespace Bictionary.Models;

public class Word
{
    public int Id { get; set; }
    public string Text { get; set; } = "";

    public string PartOfSpeech { get; set; } = "";

    public string Definition { get; set; } = "";

    public string Example { get; set; } = "";
}