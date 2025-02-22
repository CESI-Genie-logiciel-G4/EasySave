using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EasySave.Models;
using EasySave.Utils;
using EasySave.ViewModels;
using MenuItem = Avalonia.Controls.MenuItem;

namespace EasySave.Views.app;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private bool _menuItemClicked;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        AdjustMenuForMacOS();
    }
    
    public void OnStartJobButton (object? sender, RoutedEventArgs e)
    {
        var job = (BackupJob) ((Button) sender!)?.DataContext!;
        var index = _viewModel.BackupJobs.IndexOf(job);
        _viewModel.ExecuteJob(index);
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
        _viewModel.AddBackupJob(name, source, target, backupType, true);
        
        JobName.Text = "";
        SourcePath.Text = "";
        TargetPath.Text = "";
        
        SidebarTabs.SelectedIndex = 0;
    }

    private async Task<IReadOnlyList<IStorageFolder>> AskFolder()
    {
        var topLevel = GetTopLevel(this)!;

        var files = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions {
            Title = "Sélectionnez un dossier"
        });
        
        if (files.Count == 0)
        {
            throw new Exception("No folder selected");
        }
        
        return files;
    }

    private async void OnSelectTargetFolder(object sender, RoutedEventArgs e)
    {
        try
        {
            var files = await AskFolder();
            TargetPath.Text = files[0].Path.AbsolutePath;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
        }
    }
    
    private async void OnSelectSourceFolder(object sender, RoutedEventArgs e)
    {
        try
        {
            var files = await AskFolder();
            SourcePath.Text = files[0].Path.AbsolutePath;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
        }
    }
    
    private void OnMenuItemPointerPressed(object? sender, RoutedEventArgs e)
    {
        _menuItemClicked = true;
    }

    private void OnMenuPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_menuItemClicked)
        {
            _menuItemClicked = false;
            return;
        }
        BeginMoveDrag(e);
    }
    
    private void OnLanguageChanged(object sender, RoutedEventArgs e)
    {
        var menuItem = (MenuItem)sender;

        if (menuItem.DataContext is LanguageItem selectedLanguage)
        {
            _viewModel.ChangeLanguage(selectedLanguage);
        }
    }
    
    private void OnLogsTransporterChanged(object sender, RoutedEventArgs e)
    {
        var menuItem = (MenuItem)sender;

        if (menuItem.DataContext is not TransporterItem selectedTransporter) return;
        
        List<int> indexes = [_viewModel.LogTransporters.IndexOf(selectedTransporter)+1];
        _viewModel.ChangeLogsTransporters(indexes);
    }
    
    private void AdjustMenuForMacOS()
    {
        if (OperatingSystem.IsMacOS())
        {
            MainMenu.Padding = new Thickness(70, 0, 0, 0);
        }
    }
    
    private void OnClose(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}