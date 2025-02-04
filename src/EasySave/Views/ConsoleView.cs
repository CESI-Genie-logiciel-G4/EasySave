namespace EasySave.Views;

using ViewModels;
using Models;

public class ConsoleView
{
    private readonly MainViewModel _viewModel;
    public ConsoleView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.BackupJobAdded += OnBackupJobAdded;
    }
    public void Render()
    {
        Console.WriteLine("========================");
        Console.WriteLine($"  {_viewModel.Title}");
        Console.WriteLine("========================\n");
        
        foreach (var job in _viewModel.BackupJobs)
        {
            Console.WriteLine($"- {job}");
        }
        
        Console.WriteLine("\n1. Add a new job");
        Console.WriteLine("2. Exit");
        Console.Write("\nChoice: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
            {
                Console.Write("Name of the job: ");
                var jobName = Console.ReadLine() ?? "No name";
                _viewModel.AddBackupJob(jobName);
                break;
            }
            case "2":
                break;
        }
    }

    private void OnBackupJobAdded(BackupJob newJob)
    {
        Console.WriteLine($"\tNew job added: {newJob}");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Render();
    }
}