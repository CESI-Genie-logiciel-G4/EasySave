using System.Globalization;

namespace EasySave.Tests.Services;
using EasySave.Services;

public class LocalizationServiceTests
{
    [Fact]
    public void ShouldReturnTheKeyWhenTranslationIsMissing()
    {
        const string key = "fakeKey";
        var value = LocalizationService.GetString(key);
        Assert.Equal($"{key}", value);
    }
    
    [Fact]
    public void ShouldSetLanguage()
    {
        const string cultureCode = "fr";
        LocalizationService.SetLanguage(cultureCode);
        
        Assert.Equal(cultureCode, CultureInfo.CurrentUICulture.Name);
        Assert.Equal(cultureCode, CultureInfo.CurrentCulture.Name);
    }

    [Fact(Skip = "must fix wording translation")]
    public void ShouldReturnTranslationWhenKeyIsFound()
    {
        const string key = "MenuExit";
        var value = LocalizationService.GetString(key);
        Assert.Equal("Exit", value);
    }
}