namespace EasySave.Tests.Helpers;
using EasySave.Helpers;

public class ConsoleHelperTests
{
    [Fact(Skip = "must fix wording translation")]
    public void ConsolePauseTest()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        var stringReader = new StringReader("s");
        Console.SetIn(stringReader);
        
        ConsoleHelper.Pause();
        
        Assert.Contains("Press any key to continue", stringWriter.ToString());
    }
    
    [Fact(Skip = "must fix wording translation")]
    public void ConsoleMotdTest()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        var version = new Version(1, 2);
        
        ConsoleHelper.DisplayMotd(version);
        
        Assert.Contains("Version: 1.2", stringWriter.ToString());
    }
    
    [Fact]
    public void ConsoleSeparatorTest()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        ConsoleHelper.DisplaySeparator();
        
        Assert.Contains(new string('-',10), stringWriter.ToString());
    }
    
    [Fact(Skip = "must be updated")]
    public void ShouldConvertAskingNumberToInteger()
    {
        const string input = "42";
        var stringReader = new StringReader(input);
        Console.SetIn(stringReader);
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        var actualInt = ConsoleHelper.AskForInt("Enter an int", 1, 100);
        
        Assert.Equal(42, actualInt);
    }
    
    [Fact(Skip = "must fix wording translation")]
    public void ShouldAcceptsStringsOfXChars()
    {
        const string input = "Hello";
        var stringReader = new StringReader(input);
        Console.SetIn(stringReader);
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        var actualString = ConsoleHelper.AskForString("Enter a string");
        
        Assert.Equal(input, actualString);
    }

    [Theory]
    [InlineData("3", new[] {3})]
    [InlineData("1-3", new[] {1, 2, 3})]
    [InlineData("2-2", new[] {2})]
    [InlineData("1;2;4", new[] {1, 2, 4})]
    [InlineData("1;3;3", new[] {1,3})]
    [InlineData("*", new[] {1, 2, 3, 4})]
    public void ShouldConvertAskingNumbersToIntegerArray(string input, int[] expectedArray)
    {
        var stringReader = new StringReader(input);
        Console.SetIn(stringReader);
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        var actualArray = ConsoleHelper.AskForMultipleValues("Enter an array of int", 1,4);
        
        Assert.Equal(expectedArray, actualArray);
    }
    
}