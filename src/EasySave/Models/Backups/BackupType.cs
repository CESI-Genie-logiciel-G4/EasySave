using System.Text.Json.Serialization;

namespace EasySave.Models.Backups;

[JsonDerivedType(typeof(FullBackup), typeDiscriminator: "full")]
[JsonDerivedType(typeof(DifferentialBackup), typeDiscriminator: "differential")]
[JsonDerivedType(typeof(SyntheticFullBackup), typeDiscriminator: "synthetic-full")]
public abstract class BackupType(string name)
{
    public string Name { get; } = name;
    
    public abstract void Initialize(BackupJob job);
    public abstract void Execute(string sourceFile, string destinationFile, BackupJob job);
}