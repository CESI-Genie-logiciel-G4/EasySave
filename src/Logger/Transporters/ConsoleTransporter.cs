using Logger.LogEntries;

namespace Logger.Transporters;

public class ConsoleTransporter : Transporter
{
    public override void Write(ILogEntry logEntry)
    {
        Console.WriteLine(logEntry.ToString());
    }
}