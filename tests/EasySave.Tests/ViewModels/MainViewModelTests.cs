using EasySave.ViewModels;
namespace EasySave.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public void AddBackupJobTest()
    {
        var viewModel = new MainViewModel();
        
        const string expectedName = "New-Backup";
        viewModel.AddBackupJob(expectedName);

        var lastJob = viewModel.BackupJobs[^1];
        
        Assert.Equal(expectedName, lastJob.Name);
    }
}