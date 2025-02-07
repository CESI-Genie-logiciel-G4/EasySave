using System.Text.Json.Serialization;

namespace EasySave.Models;

[JsonDerivedType(typeof(FullBackup), typeDiscriminator: "full")]
[JsonDerivedType(typeof(DifferentialBackup), typeDiscriminator: "differential")]
public abstract class BackupType(string name)
{
    public string Name { get; } = name;
    public abstract void Execute(string sourceFile, string destinationFile, Execution execution, BackupJob job);
}