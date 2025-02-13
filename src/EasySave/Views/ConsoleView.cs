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
    private readonly ConsoleHelper _consoleHelper;
    private bool _isRunning = true;
    
    private readonly List<BackupJob> _backupJobs;
    
    private readonly Func<string, string> _translate;

    public ConsoleView(MainViewModel viewModel, JobService jobService, LocalizationService localizationService, ConsoleHelper consoleHelper)
    {
        _menuItems =
        [
            new MenuItem("MenuExecuteJobs", ExecuteJobs),
            new MenuItem("MenuAddJob", AddJob),
            new MenuItem("MenuRemoveJob", RemoveJob),
            new MenuItem("MenuChangeLanguage", DisplayLanguageMenu),
            new MenuItem("MenuLogsFormat", ChangeLogsFormat),
            new MenuItem("MenuExit", ExitApp)
        ];
        
        _backupJobs = jobService.BackupJobs;
        _translate = localizationService.GetString;
        _consoleHelper = consoleHelper;
        
        _viewModel = viewModel;
        _viewModel.BackupJobAdded += DisplayJobAdded;
        _viewModel.BackupJobRemoved += DisplayJobRemoved;
        
        _viewModel.ProgressUpdated += DisplayProgress;
        _viewModel.ErrorOccurred += DisplayError;
        _viewModel.LogsTransportersChanged += DisplayLogsTransportersChanged;
    }

    public void Render()
    {
        while (_isRunning)
        {
            _consoleHelper.Clear();
            _consoleHelper.DisplayMotd(VersionManager.Version);

            DisplayJobs();
            Console.WriteLine();
            DisplayMenu();

            var mainMenu = true;

            try
            {
                var choice = _consoleHelper.AskForInt(_translate("SelectOption"), 1, _menuItems.Count);
                mainMenu = false;
                _menuItems[choice - 1].Action();
            }
            catch (OperationCanceledException)
            {
                if (mainMenu) ExitApp();
                else Console.WriteLine(_translate("Exiting"));
            }

            _consoleHelper.Pause();
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine(_translate("MenuTitle"));
        for (var i = 0; i < _menuItems.Count; i++)
        {
            var item = _menuItems[i];
            Console.WriteLine($"\t{i + 1}. {_translate(item.Title)}");
        }

        Console.WriteLine();
    }

    private void DisplayJobs()
    {
        if (_backupJobs.Count == 0)
        {
            Console.WriteLine(_translate("NoJobsAvailable"));
            _consoleHelper.DisplaySeparator();
            return;
        }

        Console.WriteLine(_translate("JobsAvailable") + "\n");

        const string lineTemplate = "\t{0,-5} | {1,-20} | {2,-30} | {3,-30} | {4,-20}";
        var header = string.Format(lineTemplate, _translate("Number"), _translate("Name"), _translate("SourcePath"), _translate("DestinationPath"), _translate("Type"));
        
        Console.WriteLine(header);
        
        for (var i = 0; i < _backupJobs.Count; i++)
        {
            var job = _backupJobs[i];
            var sourceEllipsis = StringHelper.GetEllipsisSuffix(job.SourceFolder);
            var destinationEllipsis = StringHelper.GetEllipsisSuffix(job.DestinationFolder);
            
            Console.WriteLine(lineTemplate, i + 1, job.Name, sourceEllipsis, destinationEllipsis, _translate(job.BackupType.Name));
        }
    }

    private void ExecuteJobs()
    {
        var jobCount = _backupJobs.Count;

        if (jobCount == 0)
        {
            Console.WriteLine(_translate("NoJobsAvailable"));
            return;
        }

        var value = _consoleHelper.AskForMultipleValues(_translate("SelectJobsToExecute"), 1, jobCount);
        foreach (var item in value)
        {
            _viewModel.ExecuteJob(item - 1);
        }
    }

    private void AddJob()
    {
        if (_backupJobs.Count >= JobService.BackupJobLimit)
        {
            Console.WriteLine(_translate("JobLimitReached"));
            return;
        }
        var name = _consoleHelper.AskForString(_translate("EnterJobName"), 3, 50);
        var source = _consoleHelper.AskForPath(_translate("EnterSourceDirectory"));
        var destination = _consoleHelper.AskForPath(_translate("EnterDestinationDirectory"));
        
        for (var i = 0; i < _viewModel.BackupTypes.Count; i++)
        {
            var backupType = _viewModel.BackupTypes.ElementAt(i);
            Console.WriteLine($"\t{i + 1}. {backupType.Key}");
        }
        var typeChoice = _consoleHelper.AskForInt(_translate("SelectBackupType"), 1, _viewModel.BackupTypes.Count);
        var type = _viewModel.BackupTypes.ElementAt(typeChoice - 1).Value;
        
        _viewModel.AddBackupJob(name, source, destination, type);
    }

    private void RemoveJob()
    {
        var jobCount = _backupJobs.Count;

        if (jobCount == 0)
        {
            Console.WriteLine(_translate("NoJobsAvailable"));
            return;
        }

        var value = _consoleHelper.AskForInt(_translate("SelectJobToRemove"), 1, jobCount);
        _viewModel.RemoveJob(value - 1);
    }

    private void ChangeLogsFormat()
    {
        Console.WriteLine(_translate("SelectLogsTransporters"));
        for (var i = 0; i < _viewModel.LogTransporters.Count; i++)
        {
            var transporter = _viewModel.LogTransporters[i];
            var check = transporter.IsEnabled ? "[X]" : "[ ]";
            Console.WriteLine($"\t{i + 1}. {check} {_translate(transporter.Title)}");
        }
        var values = _consoleHelper.AskForMultipleValues(_translate("SelectLogsTransporters"), 1, _viewModel.LogTransporters.Count);
        _viewModel.ChangeLogsTransporters(values);
    }

    private void DisplayJobAdded(BackupJob job)
    {
        Console.WriteLine($"\t- {string.Format(_translate("JobAdded"), job.Name)}");
    }

    private void DisplayJobRemoved(int index)
    {
        Console.WriteLine($"\t- {string.Format(_translate("JobRemoved"), index + 1)}");
    }
    
    private void DisplayLanguageMenu()
    {
        Console.WriteLine(_translate("SelectLanguage"));
        for (var i = 0; i < _viewModel.Languages.Count; i++)
        {
            var language = _viewModel.Languages[i];
            Console.WriteLine($"\t{i + 1}. {_translate(language.Language)}");
        }
    
        var choice = _consoleHelper.AskForInt(_translate("SelectOption"), 1, _viewModel.Languages.Count);
   
        _viewModel.ChangeLanguage(_viewModel.Languages[choice - 1]);
    }
    
    private void DisplayProgress(Execution execution)
    {
        if (execution.State == ExecutionState.Pending || execution.TotalSteps == 0)
        {
            var link = execution.BackupJob.SourceFolder + " -> " + execution.BackupJob.DestinationFolder;
            Console.WriteLine($"\t - {_translate("StartBackupJob")} {execution.BackupJob.Name} [{link}]");
        }
        
        if (execution.TotalSteps == 0)
        {
            Console.Write($"\r\t{_translate("Progress")} [0/0] 0%");
            return;
        }
        
        if (execution.State == ExecutionState.Completed)
        {
            var message = string.Format(_translate("JobExecuted"), execution.BackupJob.Name);
            Console.WriteLine($" - {message}");
            return;
        }

        var currentStep = execution.CurrentProgress;
        var totalSteps = execution.TotalSteps;
        var percentage = (int)((double)currentStep / totalSteps * 100);
        var progressText = _translate("Progress");

        Console.Write($"\r\t{progressText} [{currentStep}/{totalSteps}] {percentage}%");
    }
    
    private void DisplayError(Exception e)
    {
        var message = e switch
        {
            UnauthorizedAccessException => _translate("UnauthorizedAccess"),
            DirectoryNotFoundException => _translate("DirectoryNotFound"),
            OperationCanceledException => _translate("OperationCancelled"),
            IOException => _translate("IOError") + " - " + _translate("CheckLogs"),
            _ => _translate("ErrorOccurred") + " - " + _translate("CheckLogs")
        };
        
        _consoleHelper.DisplayError(message, false);
    }
    
    private void DisplayLogsTransportersChanged()
    {
        Console.WriteLine(_translate("LogsFormatChanged"));
    }
    
    private void ExitApp()
    {
        Console.WriteLine(_translate("Exiting"));
        _isRunning = false;
    }
}