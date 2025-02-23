using System.Text.Json;
using EasySave.Helpers;
using EasySave.Resources;

namespace EasySave.Services;

public class SettingsService : IDisposable
{
    private static SettingsService? _instance;
    private SettingsService() {}
    public static SettingsService GetInstance()
    {
        if (_instance != null) return _instance;
        {
            LoadSettings();
            _instance = new SettingsService();
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(Path.GetFullPath(SettingsFile)) ?? string.Empty) 
                { Filter = Path.GetFileName(Path.GetFullPath(SettingsFile)), NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size };
        }
        return _instance ??= new SettingsService();
    }
    
    private static readonly JsonSerializerOptions? DefaultJsonOptions = new JsonSerializerOptions { WriteIndented = true };
    private const string SettingsFile = ".easysave/settings.json";
    private static FileSystemWatcher? _watcher;
    private static AppSettings _appSettings = new AppSettings();
    
    public AppSettings Settings => _appSettings;
    
    
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
            if (_watcher != null) _watcher.Changed += OnSettingsFileChanged;
            if (_watcher != null) _watcher.EnableRaisingEvents = true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Impossible to load settings file: { e.Message }");
        }
    }

    private static void SaveSettings()
    {
        var jsonSettings = JsonSerializer.Serialize(_appSettings, DefaultJsonOptions);
        FileHelper.CreateAndWrite(SettingsFile, jsonSettings);
    }
    
    public void UpdateLanguage(String languageCode)
    {
        if (languageCode == _appSettings.Language) return;
        _appSettings.Language = languageCode;
        SaveSettings();
    }
    
    public void UpdatePriorityExtensions(List<String> priorityExtensions)
    {
        if (priorityExtensions == _appSettings.PriorityExtensions) return;
        _appSettings.PriorityExtensions = priorityExtensions;
        SaveSettings();
    }
    
    public void Dispose()
    {
        _watcher?.Dispose();
    }
}