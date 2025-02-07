using Logger.LogEntries;

namespace Logger.Transporters;

public abstract class Transporter
{
    public abstract void Write(ILogEntry logEntry);
}