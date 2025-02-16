using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Logger.Helpers;
using Logger.LogEntries;

namespace Logger.Transporters;

public class FileXmlTransporter(string logRepositoryPath) : Transporter
{
    public override void Write(ILogEntry logEntry)
    {
        var logFilePath = FileHelper.GetLogFilePath(logRepositoryPath, ".xml");
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