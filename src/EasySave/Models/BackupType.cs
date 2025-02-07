using System.Text.Json.Serialization;
using EasySave.Converters;

namespace EasySave.Models;

[JsonConverter(typeof(BackupTypeJsonConverter))]
public abstract class BackupType(string name)
{
    public string Name { get; } = name;
    public abstract void Execute(string sourceFile, string destinationFile, Execution execution, BackupJob job);
}