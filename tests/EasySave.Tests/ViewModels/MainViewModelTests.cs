using EasySave.Models;
using EasySave.Services;
using EasySave.ViewModels;
namespace EasySave.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public void AddBackupJobTest()
    {
        var viewModel = new MainViewModel();
        
        const string expectedName = "New-Backup";
        viewModel.AddBackupJob(expectedName, "","",new FullBackup());

        var lastJob = JobService.BackupJobs[^1];
        
        Assert.Equal(expectedName, lastJob.Name);
    }
}