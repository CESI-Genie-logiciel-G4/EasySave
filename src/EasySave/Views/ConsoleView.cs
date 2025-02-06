using EasySave.Helpers;
using EasySave.Utils;
using EasySave.ViewModels;
using EasySave.Models;
using EasySave.Services;

namespace EasySave.Views;

public class ConsoleView
{
    private readonly MainViewModel _viewModel;
    private readonly List<MenuItem> _menuItems;
    private bool _isRunning = true;
    
    private static string T(string key) => LocalizationService.GetString(key);
    
    public ConsoleView(MainViewModel viewModel)
    {
        _menuItems =
        [
            new MenuItem("MenuExecuteJobs", ExecuteJobs),
            new MenuItem("MenuAddJob", AddJob),
            new MenuItem("MenuRemoveJob", RemoveJob),
            new MenuItem("MenuChangeLanguage", DisplayLanguageMenu),
            new MenuItem("MenuExit", ExitApp)
        ];
        
        _viewModel = viewModel;
        _viewModel.BackupJobExecuted += DisplayJobExecuted;
        _viewModel.BackupJobAdded += DisplayJobAdded;
        _viewModel.BackupJobRemoved += DisplayJobRemoved;
    }
    
    public void Render()
    {
        while (_isRunning)
        {
            Console.Clear();
            ConsoleHelper.DisplayMotd(VersionManager.Version);
            
            DisplayJobs();
            Console.WriteLine(ConsoleHelper.Separator);

            DisplayMenu();
            
            var choice = ConsoleHelper.AskForInt(T("SelectOption"), 1, _menuItems.Count);
            _menuItems[choice - 1].Action();
            ConsoleHelper.Pause();
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine(T("MenuTitle"));
        for (var i = 0; i < _menuItems.Count; i++)
        {
            var item = _menuItems[i];
            Console.WriteLine($"\t{i + 1}. {T(item.Title)}");
        }
        Console.WriteLine();
    }
    
    private void DisplayJobs()
    {
        if (_viewModel.BackupJobs.Count == 0)
        {
            Console.WriteLine(T("NoJobsAvailable"));
            return;
        }
        
        Console.WriteLine(T("JobsAvailable"));
        
        for (var i = 0; i < _viewModel.BackupJobs.Count; i++)
        {
            var job = _viewModel.BackupJobs[i];
            Console.WriteLine($"\t[N°{i + 1}] {job.Name}");
        }
    }
    
    private void ExecuteJobs()
    {
        var jobCount = _viewModel.BackupJobs.Count;
        
        if (jobCount == 0)
        {
            Console.WriteLine(T("NoJobsAvailable"));
            return;
        }
        
        var value = ConsoleHelper.AskForMultipleValues(T("SelectJobsToExecute"), 1, jobCount);
        foreach (var item in value)
        {
            _viewModel.ExecuteJob(item - 1);
        }
    }
    
    private void AddJob()
    {
        var name = ConsoleHelper.AskForString(T("EnterJobName"), 3, 50);
        _viewModel.AddBackupJob(name);
    }

    private void RemoveJob()
    {   
        var jobCount = _viewModel.BackupJobs.Count;
        
        if (jobCount == 0)
        {
            Console.WriteLine(T("NoJobsAvailable"));
            return;
        }
        
        var value = ConsoleHelper.AskForInt(T("SelectJobToRemove"), 1, jobCount);
        _viewModel.RemoveJob(value - 1);
    }
    
    private void DisplayJobExecuted(BackupJob job)
    {
        Console.WriteLine($"\t- {string.Format(T("JobExecuted"), job.Name)}");
    }
    
    private void DisplayJobAdded(BackupJob job)
    {
        Console.WriteLine($"\t- {string.Format(T("JobAdded"), job.Name)}");
    }
    
    private void DisplayJobRemoved(int index)
    {
        Console.WriteLine($"\t- {string.Format(T("JobRemoved"), index + 1)}");
    }
    
    private void DisplayLanguageMenu()
    {
        Console.WriteLine(T("SelectLanguage"));
        for(var i = 0; i < _viewModel.Languages.Count; i++)
        {
            var language = _viewModel.Languages[i];
            Console.WriteLine($"\t{i + 1}. {T(language.Language)}");
        }
        
        var choice = ConsoleHelper.AskForInt(T("SelectOption"), 1, _viewModel.Languages.Count);
        
        _viewModel.ChangeLanguage(_viewModel.Languages[choice - 1]);
    }
    
    private void ExitApp()
    {
        Console.WriteLine(T("Exiting"));
        _isRunning = false;
    }
}