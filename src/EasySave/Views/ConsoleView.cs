using EasySave.Helpers;
using EasySave.Utils;

namespace EasySave.Views;

using ViewModels;
using Models;
using Utils;

public class ConsoleView
{
    private readonly MainViewModel _viewModel;
    
    private readonly List<MenuItem> _menuItems;
    
    private bool _isRunning = true;
    
    public ConsoleView(MainViewModel viewModel)
    {
        _menuItems =
        [
            new MenuItem("Execute one or more jobs", ExecuteJobs),
            new MenuItem("Add a new job", AddJob),
            new MenuItem("Remove a job", RemoveJob),
            new MenuItem("Exit", ExitApp)
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
            Console.WriteLine(ConsoleHelper.Motd);
            
            DisplayJobs();
            Console.WriteLine(ConsoleHelper.Separator);

            DisplayMenu();
            
            var choice = ConsoleHelper.AskForInt("Select an option" , 1, _menuItems.Count);
            _menuItems[choice - 1].Action();
            ConsoleHelper.Pause();
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("Menu:");
        for(var i = 0; i < _menuItems.Count; i++)
        {
            var item = _menuItems[i];
            Console.WriteLine($"\t{i + 1}. {item.Title}");
        }
        Console.WriteLine();
    }
    
    private void DisplayJobs()
    {
        if (_viewModel.BackupJobs.Count == 0)
        {
            Console.WriteLine(" No jobs available");
            return;
        }
        
        Console.WriteLine(" Jobs available:");
        
        for(var i = 0; i < _viewModel.BackupJobs.Count; i++)
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
            Console.WriteLine(" No jobs to execute");
            return;
        }
        
        var value = ConsoleHelper.AskForMultipleValues("Select the jobs to execute", 1, jobCount);
        foreach (var item in value)
        {
            _viewModel.ExecuteJob(item - 1);
        }
    }
    
    private void AddJob()
    {
        var name = ConsoleHelper.AskForString("Enter the name of the job", 3, 50);
        _viewModel.AddBackupJob(name);
    }

    private void RemoveJob()
    {   
        var jobCount = _viewModel.BackupJobs.Count;
        
        if (jobCount == 0)
        {
            Console.WriteLine(" No jobs to remove");
            return;
        }
        
        var value = ConsoleHelper.AskForInt("Select the job to remove", 1, jobCount);
        _viewModel.RemoveJob(value - 1);
    }
    
    private void DisplayJobExecuted(BackupJob job)
    {
        Console.WriteLine($"\t- Job {job.Name} executed");
    }
    
    private void DisplayJobAdded(BackupJob job)
    {
        Console.WriteLine($"\t- Job {job.Name} added");
    }
    
    private void DisplayJobRemoved(int index)
    {
        Console.WriteLine($"\t- Job N°{index + 1} removed");
    }
    
    private void ExitApp()
    {
        Console.WriteLine("Exiting...");
        _isRunning = false;
    }
}