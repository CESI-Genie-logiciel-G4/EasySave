namespace Logger.Tests;

public class LoggerTest
{
    [Fact]
    public void LogTest()
    {
        const string expectedMessage = "Hello, Logger!";
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        Logger.Log(expectedMessage);
        
        var consoleOutput = stringWriter.ToString().Trim();
        Assert.Equal(expectedMessage, consoleOutput);
        
        stringWriter.Close();
    }
}