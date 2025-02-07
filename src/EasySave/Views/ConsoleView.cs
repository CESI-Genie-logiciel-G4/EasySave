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
        
        _viewModel.Notification += DisplayNotification;
        _viewModel.ProgressUpdated += DisplayProgress;
    }

    public void Render()
    {
        while (_isRunning)
        {
            ConsoleHelper.Clear();
            ConsoleHelper.DisplayMotd(VersionManager.Version);

            DisplayJobs();
            Console.WriteLine();
            DisplayMenu();

            var mainMenu = true;

            try
            {
                var choice = ConsoleHelper.AskForInt(T("SelectOption"), 1, _menuItems.Count);
                mainMenu = false;
                _menuItems[choice - 1].Action();
            }
            catch (OperationCanceledException)
            {
                if (mainMenu) ExitApp();
                else Console.WriteLine(T("Exiting"));
            }

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
            ConsoleHelper.DisplaySeparator();
            return;
        }

        Console.WriteLine(T("JobsAvailable") + "\n");

        const string lineTemplate = "\t{0,-5} | {1,-20} | {2,-30} | {3,-30} | {4,-20}";
        var header = string.Format(lineTemplate, T("Number"), T("Name"), T("SourcePath"), T("DestinationPath"), T("Type"));
        
        Console.WriteLine(header);
        
        for (var i = 0; i < _viewModel.BackupJobs.Count; i++)
        {
            var job = _viewModel.BackupJobs[i];
            var sourceEllipsis = StringHelper.GetEllipsisSuffix(job.SourceFolder, 27);
            var destinationEllipsis = StringHelper.GetEllipsisSuffix(job.DestinationFolder, 27);
            
            Console.WriteLine(lineTemplate, i + 1, job.Name, sourceEllipsis, destinationEllipsis, T(job.BackupType.Name));
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
        
        if (_viewModel.BackupJobs.Count >= MainViewModel.BackupJobLimit)
        {
            Console.WriteLine(T("JobLimitReached"));
            return;
        }
        
        var name = ConsoleHelper.AskForString(T("EnterJobName"), 3, 50);
        var source = ConsoleHelper.AskForPath(T("EnterSourceDirectory"));
        var destination = ConsoleHelper.AskForPath(T("EnterDestinationDirectory"));
        
        for (var i = 0; i < _viewModel.BackupTypes.Count; i++)
        {
            var backupType = _viewModel.BackupTypes.ElementAt(i);
            Console.WriteLine($"\t{i + 1}. {backupType.Key}");
        }
        var typeChoice = ConsoleHelper.AskForInt(T("SelectBackupType"), 1, _viewModel.BackupTypes.Count);
        var type = _viewModel.BackupTypes.ElementAt(typeChoice - 1).Value;
        
        _viewModel.AddBackupJob(name, source, destination, type);
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
        for (var i = 0; i < _viewModel.Languages.Count; i++)
        {
            var language = _viewModel.Languages[i];
            Console.WriteLine($"\t{i + 1}. {T(language.Language)}");
        }
    
        var choice = ConsoleHelper.AskForInt(T("SelectOption"), 1, _viewModel.Languages.Count);
   
        _viewModel.ChangeLanguage(_viewModel.Languages[choice - 1]);
    }
    
    private void DisplayProgress(int progress, int total)
    {
        var percentage = total == 0 ? 0 : (int)((double)progress / total * 100);
        Console.Write($"\r\t{T("Process")} [{progress}/{total}] {percentage}%");
    }
    
    private void DisplayNotification(string message, bool isError = false)   
    {
        if (isError)
        {
            ConsoleHelper.DisplayError(message, false);
            return;
        }
        
        Console.WriteLine($"\t- {message}");
    }
    
    private void ExitApp()
    {
        Console.WriteLine(T("Exiting"));
        _isRunning = false;
    }
}