using Avalonia.Controls;
using Avalonia.Interactivity;
using EasySave.Models;
using EasySave.ViewModels;

namespace EasySave.Views.app;

public partial class MainWindow : Window
{
    private MainViewModel ViewModel => DataContext as MainViewModel ?? throw new InvalidOperationException("DataContext is not MainViewModel");
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnAddJob(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("Add job clicked");
    }

    private void OnRemoveJob(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("Remove job clicked");
    }

    private void OnExecuteJobs(object? sender, RoutedEventArgs e)
    {
        ViewModel.Executions.Clear();
        System.Console.WriteLine("Execute job clicked");
        
        foreach (var job in JobList.SelectedItems)
        {
            var index = JobList.Items.IndexOf(job);
            ViewModel.ExecuteJob(index);
        }
    }

    private void OnChangeLogsFormat(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("Change logs format clicked");
    }

    private void OnBackupJobAdded(BackupJob job)
    {
        System.Console.WriteLine("Job added");
    }

    private void OnBackupJobRemoved(int index)
    {
        System.Console.WriteLine("Job removed");
    }
    
    private void OnErrorOccurred(Exception e)
    {
        var message = e switch
        {
            UnauthorizedAccessException => "Access denied. Please check permissions.",
            DirectoryNotFoundException => "Directory not found. Please verify the path.",
            OperationCanceledException => "Operation was cancelled.",
            IOException => "I/O error occurred. Please check the logs for details.",
            _ => "An error occurred. Please check the logs for details."
        };
        
        System.Console.WriteLine(message);
    }
}