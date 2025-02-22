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
    private readonly List<MenuItem> _extensionsMenuItems;
    private bool _isRunning = true;
    
    private readonly List<BackupJob> _backupJobs = JobService.BackupJobs;
    
    private static string T(string key) => LocalizationService.GetString(key);

    public ConsoleView(MainViewModel viewModel)
    {
        _menuItems =
        [
            new MenuItem("MenuExecuteJobs", ExecuteJobs),
            new MenuItem("MenuAddJob", AddJob),
            new MenuItem("MenuRemoveJob", RemoveJob),
            new MenuItem("MenuEncryptedExtensions", HandleEncryptedExtensions),
            new MenuItem("MenuChangeLanguage", DisplayLanguageMenu),
            new MenuItem("MenuLogsFormat", ChangeLogsFormat),
            new MenuItem("MenuExit", ExitApp)
        ];
        _extensionsMenuItems =
        [
            new MenuItem("MenuAddExtension", AddExtension),
            new MenuItem("MenuRemoveExtension", RemoveExtension),
            new MenuItem("MenuReturn", null!)
        ];

        _viewModel = viewModel;
        _viewModel.BackupJobAdded += DisplayJobAdded;
        _viewModel.BackupJobRemoved += DisplayJobRemoved;
        
        _viewModel.ProgressUpdated += DisplayProgress;
        _viewModel.ErrorOccurred += DisplayError;
        _viewModel.LogsTransportersChanged += DisplayLogsTransportersChanged;
        
        _viewModel.EncryptedExtensionsChanged += DisplayEncryptedExtensionsChanged;
        _viewModel.ExtensionsAdded += DisplayExtensionsAdded;
        _viewModel.ExtensionsRemoved += DisplayExtensionsRemoved;
        
    }

    public void Render()
    {
        while (_isRunning)
        {
            ConsoleHelper.Clear();
            ConsoleHelper.DisplayMotd(VersionManager.Version);

            DisplayJobs();
            Console.WriteLine();
            DisplayMenu(_menuItems);

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

    private void DisplayMenu(List<MenuItem> menuItems)
    {
        Console.WriteLine(T("MenuTitle"));
        for (var i = 0; i < menuItems.Count; i++)
        {
            var item = menuItems[i];
            Console.WriteLine($"\t{i + 1}. {T(item.Title)}");
        }

        Console.WriteLine();
    }

    private void DisplayJobs()
    {
        if (_backupJobs.Count == 0)
        {
            Console.WriteLine(T("NoJobsAvailable"));
            ConsoleHelper.DisplaySeparator();
            return;
        }

        Console.WriteLine(T("JobsAvailable") + "\n");

        const string lineTemplate = "\t{0,-5} | {1,-20} | {2,-30} | {3,-30} | {4,-20} | {5,-15}";
        var header = string.Format(lineTemplate, T("Number"), T("Name"), T("SourcePath"), T("DestinationPath"), T("Type"), T("Encryption"));
        
        Console.WriteLine(header);
        
        for (var i = 0; i < _backupJobs.Count; i++)
        {
            var job = _backupJobs[i];
            var sourceEllipsis = StringHelper.GetEllipsisSuffix(job.SourceFolder, 27);
            var destinationEllipsis = StringHelper.GetEllipsisSuffix(job.DestinationFolder, 27);
            var encryption = job.UseEncryption ? T("Yes") : T("No");

            Console.WriteLine(lineTemplate, i + 1, job.Name, sourceEllipsis, destinationEllipsis, T(job.BackupType.Name), encryption);
        }
    }

    private void ExecuteJobs()
    {
        var jobCount = _backupJobs.Count;

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
        var source = ConsoleHelper.AskForPath(T("EnterSourceDirectory"));
        var destination = ConsoleHelper.AskForPath(T("EnterDestinationDirectory"));
        
        for (var i = 0; i < _viewModel.BackupTypes.Count; i++)
        {
            var backupType = _viewModel.BackupTypes.ElementAt(i);
            Console.WriteLine($"\t{i + 1}. {backupType.Key}");
        }
        var typeChoice = ConsoleHelper.AskForInt(T("SelectBackupType"), 1, _viewModel.BackupTypes.Count);
        var type = _viewModel.BackupTypes.ElementAt(typeChoice - 1).Value;
        
        var encryption = ConsoleHelper.AskForBool(T("UseEncryption"));
        _viewModel.AddBackupJob(name, source, destination, type, encryption);
    }

    private void RemoveJob()
    {
        var jobCount = _backupJobs.Count;

        if (jobCount == 0)
        {
            Console.WriteLine(T("NoJobsAvailable"));
            return;
        }

        var value = ConsoleHelper.AskForInt(T("SelectJobToRemove"), 1, jobCount);
        _viewModel.RemoveJob(value - 1);
    }

    private void ChangeLogsFormat()
    {
        Console.WriteLine(T("SelectLogsTransporters"));
        for (var i = 0; i < _viewModel.LogTransporters.Count; i++)
        {
            var transporter = _viewModel.LogTransporters[i];
            var check = transporter.IsEnabled ? "[X]" : "[ ]";
            Console.WriteLine($"\t{i + 1}. {check} {T(transporter.Title)}");
        }
        var values = ConsoleHelper.AskForMultipleValues(T("SelectLogsTransporters"), 1, _viewModel.LogTransporters.Count);
        _viewModel.ChangeLogsTransporters(values);
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
    
    private void DisplayProgress(Execution execution)
    {
        if (execution.State == ExecutionState.Pending || execution.TotalSteps == 0)
        {
            var link = execution.BackupJob.SourceFolder + " -> " + execution.BackupJob.DestinationFolder;
            Console.WriteLine($"\t - {T("StartBackupJob")} {execution.BackupJob.Name} [{link}]");
        }
        
        if (execution.TotalSteps == 0)
        {
            Console.Write($"\r\t{T("Progress")} [0/0] 0%");
            return;
        }
        
        if (execution.State == ExecutionState.Completed)
        {
            var message = string.Format(T("JobExecuted"), execution.BackupJob.Name);
            Console.WriteLine($" - {message}");
            return;
        }

        var currentStep = execution.CurrentProgress;
        var totalSteps = execution.TotalSteps;
        var percentage = (int)((double)currentStep / totalSteps * 100);
        var progressText = T("Progress");

        Console.Write($"\r\t{progressText} [{currentStep}/{totalSteps}] {percentage}%");
    }
    
    private void DisplayError(Exception e)
    {
        var message = e switch
        {
            UnauthorizedAccessException => T("UnauthorizedAccess"),
            DirectoryNotFoundException => T("DirectoryNotFound"),
            OperationCanceledException => T("OperationCancelled"),
            IOException => T("IOError") + " - " + T("CheckLogs"),
            _ => T("ErrorOccurred") + " - " + T("CheckLogs")
        };
        
        ConsoleHelper.DisplayError(message, false);
    }
    
    private void DisplayLogsTransportersChanged()
    {
        Console.WriteLine(T("LogsFormatChanged"));
    }
    
    private void HandleEncryptedExtensions()
    {
        if (_viewModel.EncryptedExtensions.Count == 0)
        {
            var extensions = ConsoleHelper.AskForStringList(T("EnterEncryptedExtensions") + ": ");
            _viewModel.SetupEncryptedExtensions(extensions);
            return;
        }

        Console.WriteLine(T("CurrentEncryptedExtensions") + ":");
        foreach (var extension in _viewModel.EncryptedExtensions)
        {
            Console.WriteLine($"\t- {extension}");
        }
        ConsoleHelper.DisplaySeparator();
        
        while (true)
        {
            DisplayMenu(_extensionsMenuItems);
            var choice = ConsoleHelper.AskForInt(T("SelectOption"), 1, _extensionsMenuItems.Count);
            if (choice == _extensionsMenuItems.Count) return;
            _extensionsMenuItems[choice - 1].Action();
        }
    }

    private void RemoveExtension()
    {
        if (_viewModel.EncryptedExtensions.Count == 0)
        {
            Console.WriteLine(T("NoExtensionsAvailable"));
            return;
        }
        Console.WriteLine(T("SelectExtensionToRemove") + ":");
        for (var i = 0; i < _viewModel.EncryptedExtensions.Count; i++)
        {
            Console.WriteLine($"\t{i + 1}. {_viewModel.EncryptedExtensions[i]}");
        }

        var choice = ConsoleHelper.AskForMultipleValues(T("SelectOption"), 1, _viewModel.EncryptedExtensions.Count);
        _viewModel.RemoveExtension(choice);
    }

    private void AddExtension()
    {
        var newExtensions = ConsoleHelper.AskForStringList(T("EnterNewExtensions") + ": ");

        foreach (var ext in newExtensions)
        {
            if (!_viewModel.EncryptedExtensions.Contains(ext))
            {
                _viewModel.AddExtensions(ext);
            }
            else
            {
                Console.WriteLine($"\t- {ext} {T("ExtensionAlreadyExists")}");
            }
        }

        _viewModel.SetupEncryptedExtensions(_viewModel.EncryptedExtensions);
    }
    
    private void DisplayEncryptedExtensionsChanged(List<string> extensions)
    {
        Console.WriteLine(T("EncryptedExtensionsChanged")+": " + string.Join(", ", extensions));
        
    }
    
    private void DisplayExtensionsAdded(string ext)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{T("ExtensionAdded")} {ext}");
        Console.ResetColor();
    }

    private void DisplayExtensionsRemoved(string ext)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{T("ExtensionRemoved")} {ext}");
        Console.ResetColor();
    }
    
    private void ExitApp()
    {
        Console.WriteLine(T("Exiting"));
        _isRunning = false;
    }
}