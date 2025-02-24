using System.Collections.ObjectModel;
using System.Text.Json;
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
    
    static SettingsService ()
    {
        LoadSettings();
        var fullSettingsPath = Path.GetFullPath(SettingsFile);
        Watcher = new FileSystemWatcher(Path.GetDirectoryName(fullSettingsPath) ?? string.Empty) 
            { Filter = Path.GetFileName(fullSettingsPath), NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size };
        Watcher.Changed += OnSettingsFileChanged;
        Watcher.EnableRaisingEvents = true;
    }
    
    private static void OnSettingsFileChanged(object sender, FileSystemEventArgs e)
    {
        LoadSettings();
    }
    
    private static void LoadSettings()
    {
        if (!File.Exists(SettingsFile))
        {
            _appSettings = new AppSettings();
            SaveSettings();
            return;
        }
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
    
    public static void UpdatePriorityExtensions(List<String> priorityExtensions)
    {
        if (priorityExtensions == _appSettings.PriorityExtensions) return;
        _appSettings.PriorityExtensions = priorityExtensions;
        SaveSettings();
    }
}