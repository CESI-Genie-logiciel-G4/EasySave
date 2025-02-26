using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using EasySave.Exceptions;
using EasySave.Helpers;
using EasySave.Resources;

namespace EasySave.Services;

public static class SettingsService
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new () { WriteIndented = true };
    private const string SettingsFile = ".easysave/settings.json";
    private static readonly FileSystemWatcher Watcher;
    private static AppSettings _appSettings = new ();
    public static AppSettings Settings => _appSettings;
    
    /// <summary>
    ///     Initialise the static classe with :
    ///      - Create the settings file if needed
    ///      - Load settings when existing
    ///      - Make a file watcher on updates
    /// </summary>
    static SettingsService ()
    {
        EnsureSettingsExists();
        
        var fullSettingsPath = Path.GetFullPath(SettingsFile);
        Watcher = new FileSystemWatcher(Path.GetDirectoryName(fullSettingsPath) ?? string.Empty) 
            { Filter = Path.GetFileName(fullSettingsPath), NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size };
        Watcher.Changed += OnSettingsFileChanged;
        Watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Ensure that there is a setting file and create it if needed with the good format
    /// </summary>
    private static void EnsureSettingsExists()
    {
        if (!File.Exists(SettingsFile)) SetDefaultSettings();
        else LoadSettings();
    }

    private static void SetDefaultSettings()
    {
        _appSettings = new AppSettings();
        var jsonSettings = JsonSerializer.Serialize(_appSettings, DefaultJsonOptions);
        FileHelper.CreateAndWrite(SettingsFile, jsonSettings);
    }
    
    private static void OnSettingsFileChanged(object sender, FileSystemEventArgs e)
    {
        LoadSettings();
    }
    
    private static void LoadSettings()
    {
        try
        {
            var json = File.ReadAllText(SettingsFile);
            _appSettings = JsonSerializer.Deserialize<AppSettings>(json, DefaultJsonOptions) ?? new AppSettings();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Impossible to load settings file: { e.Message }");
        }
    }

    private static void SaveSettings()
    {
        var jsonSettings = JsonSerializer.Serialize(_appSettings, DefaultJsonOptions);
        Watcher.EnableRaisingEvents = false;
        FileHelper.CreateAndWrite(SettingsFile, jsonSettings);
        Watcher.EnableRaisingEvents = true;
        
    }
    
    public static void UpdateLanguage(String languageCode)
    {
        if (languageCode == _appSettings.Language) return;
        _appSettings.Language = languageCode;
        SaveSettings();
    }
    
    public static void UpdatePriorityExtensions(ObservableCollection<String> priorityExtensions)
    {
        if (priorityExtensions == _appSettings.PriorityExtensions) return;
        _appSettings.PriorityExtensions = priorityExtensions;
        SaveSettings();
    }

    public static void UpdateLogTransporters(List<int> indexes)
    {
        foreach (var numericalIndex in indexes)
        {
            var transporter = _appSettings.LogTransporters.ElementAt(numericalIndex - 1);
            transporter.IsEnabled = !transporter.IsEnabled;
        }
        SaveSettings();
    }

    public static void AddEncryptExtensions(string encryptExtension)
    {
        if (_appSettings.EncryptExtensions.Contains(encryptExtension)) 
            throw new AlreadyExistException("Encrypting extension already exists in the list", encryptExtension);
        _appSettings.EncryptExtensions.Add(encryptExtension);
        SaveSettings();
    }

    public static void RemoveEncryptExtensions(string encryptExtension)
    {
        if (!_appSettings.EncryptExtensions.Contains(encryptExtension))
            throw new AlreadyExistException("Encrypting extension does not exist in the list", encryptExtension);
        _appSettings.EncryptExtensions.Remove(encryptExtension);
        SaveSettings();
    }

    public static void AddPriorityProcessNames(string priorityProcessNames)
    {
        if (_appSettings.PriorityProcessNames.Contains(priorityProcessNames))
            return;
        _appSettings.PriorityProcessNames.Add(priorityProcessNames);
        SaveSettings();
    }

    public static void RemovePriorityProcessNames(string priorityProcessNames)
    {
        if(!_appSettings.PriorityProcessNames.Contains(priorityProcessNames)) return;
        _appSettings.PriorityProcessNames.Remove(priorityProcessNames);
        SaveSettings();
    }

    public static void UpdateMaxFileSize(long size)
    {
        _appSettings.MaxFileSizeKb = size;
        SaveSettings();
    }
}