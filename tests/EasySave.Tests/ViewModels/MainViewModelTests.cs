using EasySave.Models;
using EasySave.ViewModels;
namespace EasySave.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact(Skip = "must mock file system")]
    public void AddBackupJobTest()
    {
        var viewModel = new MainViewModel();
        
        const string expectedName = "New-Backup";
        viewModel.AddBackupJob(expectedName, "","",new FullBackup());

        var lastJob = viewModel.BackupJobs[^1];
        
        Assert.Equal(expectedName, lastJob.Name);
    }
}