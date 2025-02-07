using System.Text.Encodings.Web;
using System.Text.Json;

namespace Logger.LogEntries;

public class CopyFileLogEntry(
    string backupName,
    string sourcePath,
    string destinationPath,
    long fileSize,
    double transferTimeMs)
    : ILogEntry
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string LoggedOperation { get; set; } = "Copy a file";
    public string BackupName { get; set; } = backupName;
    public string SourcePath { get; set; } = sourcePath;
    public string DestinationPath { get; set; } = destinationPath;
    public long FileSize { get; set; } = fileSize;
    public double TransferTimeMs { get; set; } = transferTimeMs;
    
    public override string ToString() => $"[{Timestamp}] - {LoggedOperation} in {TransferTimeMs} - {BackupName} backup in progress";
    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}