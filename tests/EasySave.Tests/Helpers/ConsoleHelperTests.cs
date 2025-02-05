namespace EasySave.Tests.Helpers;
using EasySave.Helpers;

public class ConsoleHelperTests
{
    [Fact]
    public void ConsolePauseTest()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        var stringReader = new StringReader("s");
        Console.SetIn(stringReader);
        
        ConsoleHelper.Pause();
        
        Assert.Equal("\nPress any key to continue...", stringWriter.ToString());
    }
    
    [Fact]
    public void ShouldConvertAskingNumberToInteger()
    {
        var input = "42";
        var stringReader = new StringReader(input);
        Console.SetIn(stringReader);
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        var actualInt = ConsoleHelper.AskForInt("Enter an int", 1, 100);
        
        Assert.Equal(42, actualInt);
    }
    
    [Fact]
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
    [InlineData("1;2;4", new[] {1, 2, 4})]
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