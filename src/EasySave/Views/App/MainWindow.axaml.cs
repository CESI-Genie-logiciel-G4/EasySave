using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EasySave.Models;
using EasySave.Services;
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
    }

    public void OnStartJobButton(object? sender, RoutedEventArgs e)
    {
        var job = (BackupJob)((Button)sender!)?.DataContext!;
        var index = _viewModel.BackupJobs.IndexOf(job);
        _viewModel.ExecuteJob(index);
    }

    public void OnDeleteMenuItemClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem) return;
        if (menuItem.DataContext is not BackupJob backupJob) return;

        var index = _viewModel.BackupJobs.IndexOf(backupJob);

        _viewModel.RemoveJob(index);
    }

    private void OnCreateJobButton(object? sender, RoutedEventArgs e)
    {
        var name = JobName.Text;
        var source = SourcePath.Text;
        var target = TargetPath.Text;
        var modeIndex = Mode.SelectedIndex;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target) ||
            modeIndex == -1)
        {
            return;
        }

        var backupType = _viewModel.BackupTypes[modeIndex];
        var isEncrypted = Encryption.IsChecked ?? false;

        _viewModel.AddBackupJob(name, source, target, backupType, isEncrypted);

        JobName.Text = "";
        SourcePath.Text = "";
        TargetPath.Text = "";

        SidebarTabs.SelectedIndex = 0;
    }

    private async Task<IReadOnlyList<IStorageFolder>> AskFolder()
    {
        var topLevel = GetTopLevel(this)!;

        var files = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
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
            TargetPath.Text = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
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
            SourcePath.Text = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
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
        var menuItem = (Button)sender;

        if (menuItem.DataContext is LanguageItem selectedLanguage)
        {
            _viewModel.ChangeLanguage(selectedLanguage);
        }
    }

    private void OnClose(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void navigateToSettings(object? sender, RoutedEventArgs e)
    {
        SidebarTabs.SelectedIndex = 3;
    }

    private void OnAddEncryptedExtension(object? sender, RoutedEventArgs e)
    {
        var newExtension = NewEncryptedExtension.Text;
        if (string.IsNullOrEmpty(newExtension))
        {
            return;
        }

        _viewModel.AddExtensions(newExtension, ExtensionService.ExtensionType.Encrypted);
        NewEncryptedExtension.Text = "";
    }

    private void OnDeleteEncryptedExtension(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem) return;
        if (menuItem.DataContext is not string selectedItem) return;

        var index = _viewModel.EncryptedExtensions.IndexOf(selectedItem);
        _viewModel.RemoveExtension([index + 1], ExtensionService.ExtensionType.Encrypted);
    }

    private void OnLogTransporterChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not TransporterItem transporterItem) return;

        var index = _viewModel.LogTransporters.IndexOf(transporterItem);
        _viewModel.ChangeLogsTransporters([index + 1]);
    }
    
    private void OnAddPriorityExtension(object? sender, RoutedEventArgs e)
    {
        var newExtension = NewPriorityExtension.Text;
        if (string.IsNullOrEmpty(newExtension))
        {
            return;
        }

        _viewModel.AddExtensions(newExtension, ExtensionService.ExtensionType.Priority);
        NewEncryptedExtension.Text = "";
    }
    
    private void OnDeletePriorityExtension(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem) return;
        if (menuItem.DataContext is not string selectedItem) return;

        var index = _viewModel.PriorityExtensions.IndexOf(selectedItem);
        _viewModel.RemoveExtension([index + 1], ExtensionService.ExtensionType.Priority);
    }
}