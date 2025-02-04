namespace EasySave.Tests.Models;

using EasySave.Models;

public class BackupJobTests
{
    [Fact]
    public void ToStringTest()
    {
        const string expectedName = "Full";
        var backupJob = new BackupJob(expectedName);
        
        var actualString = backupJob.ToString();
        
        Assert.Equal($"[BackupJob] {expectedName}", actualString);
    }
}