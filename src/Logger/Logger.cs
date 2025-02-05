using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Logger.LogEntries;
using Logger.Transporters;

namespace Logger;

public class Logger : ILogger
{
    private static Logger? _instance;
    private Logger() {}

    private List<Transporter>? _transporters;
    
    
    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Logger();
        }
        return _instance;
    }

    public void SetupTransporters(List<Transporter>? transporters)
    {
        _transporters = transporters;
    }
    
    
    public void Log(ILogEntry logEntry)
    {
        if (_transporters == null)
        {
            throw new NullReferenceException("No transporter have been provided.");
        }
        foreach (var transporter in _transporters)
        {
            transporter.Write(logEntry);
        }
    }
}