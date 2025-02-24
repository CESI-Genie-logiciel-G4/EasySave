using EasySave.Models.Backups;

namespace EasySave.Tests.Models;

using EasySave.Models;

public class BackupJobTests
{
    [Fact(Skip = "must mock file system")]
    public void ToStringTest()
    {
        const string expectedName = "Full";
        var backupJob = new BackupJob(expectedName, "", "",new FullBackup(), false);
        
        var actualString = backupJob.ToString();
        
        Assert.Equal($"[BackupJob] {expectedName}", actualString);
    }
}