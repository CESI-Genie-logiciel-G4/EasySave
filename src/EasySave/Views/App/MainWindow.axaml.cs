using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using EasySave.ViewModels;

namespace EasySave.Views.app;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        AdjustMenuForMacOS();
    }
    
    private void OnStartJobButton(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("Start job clicked");
    }
    
    private void OnCreateJobButton(object? sender, RoutedEventArgs e)
    {
        var name = JobName.Text;
        var source = SourcePath.Text;
        var target = TargetPath.Text;
        var modeIndex = Mode.SelectedIndex;
        
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target) || modeIndex == -1)
        {
            return;
        }

        var backupType = _viewModel.BackupTypes[modeIndex];
        _viewModel.AddBackupJob(name, source, target, backupType);
        
        JobName.Text = "";
        SourcePath.Text = "";
        TargetPath.Text = "";
        
        SidebarTabs.SelectedIndex = 0;
    }
 
    private async void OnSelectSourceFolder(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog { Title = "Sélectionnez un dossier source" };
        var result = await dialog.ShowAsync(this);
        if (!string.IsNullOrEmpty(result))
        {
            SourcePath.Text = result;
        }
    }

    private async void OnSelectTargetFolder(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog { Title = "Sélectionnez un dossier de destination" };
        var result = await dialog.ShowAsync(this);
        if (!string.IsNullOrEmpty(result))
        {
            TargetPath.Text = result;
        }
    }

    private void OnMenuPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
    
    private void AdjustMenuForMacOS()
    {
        if (OperatingSystem.IsMacOS())
        {
            MainMenu.Padding = new Thickness(70, 0, 0, 0);
        }
    }
}