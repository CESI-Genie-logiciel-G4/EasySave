namespace EasySave.Services;

using System.Globalization;
using System.Resources;

using System.Reflection;

public class LocalizationService
{
    private readonly ResourceManager _resourceManager = new ResourceManager("EasySave.Resources.Globalization.Strings", Assembly.GetExecutingAssembly());

    public string GetString(string key)
    {
        try {
            return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        } catch (MissingManifestResourceException) {
            return $"[Missing Translation: ({key})]";
        }
    }

    public void SetLanguage(string cultureCode)
    {
        var newCulture = new CultureInfo(cultureCode);
        CultureInfo.CurrentUICulture = newCulture;
        CultureInfo.CurrentCulture = newCulture;
    }
}