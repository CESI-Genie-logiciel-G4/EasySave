namespace EasySave.Utils;

public class MenuItem
{
    public string Title { get; }
    public Func<Task> Action { get; }
    
    public MenuItem(string title, Func<Task> action)
    {
        Title = title;
        Action = action;
    }
    
    public MenuItem(string title, Action action)
    {
        Title = title;
        Action = () => {
            action(); 
            return Task.CompletedTask;
        };
    }
}