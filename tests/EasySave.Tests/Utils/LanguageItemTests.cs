namespace EasySave.Tests.Utils;

using EasySave.Utils;

public class LanguageItemTests
{
    [Fact]
    public void TestGetLanguageItem()
    {
        var languageItem = new LanguageItem("French", "fr");
        
        Assert.Equal("French", languageItem.Language);
    }
}