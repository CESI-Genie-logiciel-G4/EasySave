using Logger.LogEntries;
using Logger.Transporters;

namespace Logger;

public interface ILogger
{
    public void SetupTransporters(List<Transporter> transporters);
    public void Log(ILogEntry logEntry);
}