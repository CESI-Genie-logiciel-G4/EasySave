using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Logger.LogEntries;

public class CopyFileLogEntry() : LogEntry("Copy a file")
{
    [JsonPropertyOrder(3)] 
    public string BackupName { get; set; } = null!;

    [JsonPropertyOrder(4)]
    public string SourcePath { get; set; } = null!;

    [JsonPropertyOrder(5)]
    public string DestinationPath { get; set; } = null!;

    [JsonPropertyOrder(6)]
    public long FileSize { get; set; }
    [JsonPropertyOrder(7)]
    public double TransferTimeMs { get; set; }
    
    public CopyFileLogEntry(string backupName, string sourcePath, string destinationPath, long fileSize, double transferTimeMs) : this()
    {
        BackupName = backupName;
        SourcePath = sourcePath;
        DestinationPath = destinationPath;
        FileSize = fileSize;
        TransferTimeMs = transferTimeMs;
    }

    public override string ToString() => $" [{Timestamp}] - {LoggedOperation} in {TransferTimeMs} ms - {BackupName} backup in progress";
}