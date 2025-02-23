using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Collections;
using Avalonia;
using Avalonia.Controls;

namespace EasySave.Services
{
    public static class LocalizationService
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("EasySave.Resources.Globalization.Strings", Assembly.GetExecutingAssembly());

        public static ResourceDictionary CurrentResourceDictionary { get; private set; } = new ResourceDictionary();
        
        public static string GetString(string key)
        {
            try {
                return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
            } catch (MissingManifestResourceException) {
                return $"[Missing Translation: ({key})]";
            }
        }
        
        public static void UpdateAvaloniaResources()
        {
            var newResourceDictionary = new ResourceDictionary();

            var resourceSet = ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            if (resourceSet != null)
            {
                foreach (DictionaryEntry entry in resourceSet)
                {
                    var key = entry.Key.ToString()!;
                    var value = entry.Value?.ToString()!;
                    newResourceDictionary[key] = value;
                }
            }

            if (Application.Current != null)
            {
                Application.Current.Resources = newResourceDictionary;
            }

            CurrentResourceDictionary = newResourceDictionary;
        }
        
        public static void SetLanguage(string cultureCode)
        {
            CultureInfo newCulture = new CultureInfo(cultureCode);
            CultureInfo.CurrentUICulture = newCulture;
            CultureInfo.CurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            SettingsService.GetInstance().UpdateLanguage(cultureCode);
        }
    }
}
