using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Logger.Helpers;
using Logger.LogEntries;

namespace Logger.Transporters;

[method: JsonConstructor]
public class FileXmlTransporter(string logRepositoryPath) : Transporter
{
    public String LogRepositoryPath { get; set; } = logRepositoryPath;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void Write(ILogEntry logEntry)
    {
        var logFilePath = FileHelper.GetLogFilePath(LogRepositoryPath, ".xml");
        FileHelper.CreateLogDirectoryIfNotExists(logFilePath);

        if (!File.Exists(logFilePath))
        {
            var streamWriter = File.CreateText(logFilePath);
            streamWriter.Close();
        }

        try
        {
            var sw = File.AppendText(logFilePath);
            using (sw)
            {
                var xml = new XmlSerializer(typeof(LogEntry));
                xml.Serialize(sw, logEntry);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}