﻿using EasySave.Helpers;
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
    
    private readonly List<BackupJob> _backupJobs = JobService.BackupJobs;
    
    private static string T(string key) => LocalizationService.GetString(key);

    public ConsoleView(MainViewModel viewModel)
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
        if (_backupJobs.Count == 0)
        {
            Console.WriteLine(T("NoJobsAvailable"));
            ConsoleHelper.DisplaySeparator();
            return;
        }

        Console.WriteLine(T("JobsAvailable") + "\n");

        const string lineTemplate = "\t{0,-5} | {1,-20} | {2,-30} | {3,-30} | {4,-20}";
        var header = string.Format(lineTemplate, T("Number"), T("Name"), T("SourcePath"), T("DestinationPath"), T("Type"));
        
        Console.WriteLine(header);
        
        for (var i = 0; i < _backupJobs.Count; i++)
        {
            var job = _backupJobs[i];
            var sourceEllipsis = StringHelper.GetEllipsisSuffix(job.SourceFolder, 27);
            var destinationEllipsis = StringHelper.GetEllipsisSuffix(job.DestinationFolder, 27);
            
            Console.WriteLine(lineTemplate, i + 1, job.Name, sourceEllipsis, destinationEllipsis, T(job.BackupType.Name));
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
        if (_backupJobs.Count >= JobService.BackupJobLimit)
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
    
    private void ExitApp()
    {
        Console.WriteLine(T("Exiting"));
        _isRunning = false;
    }
}