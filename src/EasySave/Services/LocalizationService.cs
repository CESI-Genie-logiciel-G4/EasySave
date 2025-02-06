namespace EasySave.Services;

using System.Globalization;
using System.Resources;

using System.Reflection;

public static class LocalizationService
{
    private static readonly ResourceManager ResourceManager = new ResourceManager("EasySave.Resources.Globalization.Strings", Assembly.GetExecutingAssembly());

    public static string GetString(string key)
    {
        try {
            return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        } catch (MissingManifestResourceException) {
            return $"[Missing Translation: ({key})]";
        }
    }

    public static void SetLanguage(string cultureCode)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(cultureCode);
        CultureInfo.CurrentCulture = new CultureInfo(cultureCode);
    }
}