namespace EasySave.Utils;

public class MenuItem(string title, Action action)
{
    public string Title { get; } = title;
    public Action Action { get; } = action;
}