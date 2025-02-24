using System.Text.Json.Serialization;
using Logger.LogEntries;

namespace Logger.Transporters;

[JsonDerivedType(typeof(ConsoleTransporter), typeDiscriminator: "console")]
[JsonDerivedType(typeof(FileJsonTransporter), typeDiscriminator: "json")]
[JsonDerivedType(typeof(FileXmlTransporter), typeDiscriminator: "xml")]
public abstract class Transporter
{
    public abstract void Write(ILogEntry logEntry);
}