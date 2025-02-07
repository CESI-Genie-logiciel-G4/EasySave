namespace EasySave.Utils;

public class LanguageItem(string language, string identifier)
{
    public string Language { get; } = language;
    public string Identifier { get; } = identifier;
}