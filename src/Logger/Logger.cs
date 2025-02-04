namespace Logger;

public class Logger
{
    private static Logger? _instance;
    private Logger() { }
    
    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Logger();
        }
        return _instance;
    }
    
    public static void Log(string message)
    {
        Console.WriteLine(message);
    }
}