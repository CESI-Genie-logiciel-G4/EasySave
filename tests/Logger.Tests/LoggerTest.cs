using Logger.LogEntries;
using Logger.Transporters;
using Newtonsoft.Json;

namespace Logger.Tests;

public class LoggerTest
{
    [Fact]
    public void SingletonTest()
    {
        var logger1 = Logger.GetInstance();
        var logger2 = Logger.GetInstance();
        
        Assert.Same(logger1, logger2);
    }
    
    [Fact]
    public void CopyLogByJsonTest()
    {
        //Initialize
        var logDirectory = ".";
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var logFile = Path.Combine(logDirectory, $"log_{date}.json");
        if (File.Exists(logFile))
        {
            File.Delete(logFile);
        }
        
        const string expectedOperation = "Copy a file";
        const string expectedBackupName = "save1";
        const string expectedSourcePath = @"\\UNC\source\Path";
        const string expectedDestinationPath = @"UNC\destination\Path";
        const long expectedFileSize = 12;
        const double expectedTransferTime = 3.356;
        
        var jsonTransporter = new FileJsonTransporter(logDirectory);
        var logger = Logger.GetInstance();
        logger.SetupTransporters([jsonTransporter]);
        
        CopyFileLogEntry copyLog = new CopyFileLogEntry(expectedBackupName, 
                                                        expectedSourcePath, 
                                                        expectedDestinationPath, 
                                                        expectedFileSize, 
                                                        expectedTransferTime);
        
        //Launch
        logger.Log(copyLog);
        
        //Asserts
        Assert.True(File.Exists(logFile));
        var createdFile = File.ReadAllText(logFile);
        File.Delete(logFile);
        Assert.Contains(expectedOperation, createdFile);
        Assert.Contains(expectedBackupName, createdFile);
        Assert.Contains(expectedSourcePath, createdFile);
        Assert.Contains(expectedDestinationPath, createdFile);
        Assert.Contains(expectedFileSize.ToString(), createdFile);
    }

    [Fact]
    public void GlobalLogByConsoleTest()
    {
        const string  expectedOperation = "Create backup job";
        const string expectedMessage = "No details";
        var givenMetadata = new Dictionary<string, object>
        {
            {"Backup Name", "save1"},
            {"Backup Type", "Differential"},
        };
        
        var logger = Logger.GetInstance();
        logger.SetupTransporters([new ConsoleTransporter()]);
        
        var writer = new StringWriter();
        Console.SetOut(writer);
        
        logger.Log(new GlobalLogEntry(expectedOperation, expectedMessage, givenMetadata));
        
        Assert.Contains(expectedOperation, writer.ToString());
        Assert.Contains(expectedMessage, writer.ToString());
        Assert.DoesNotContain(givenMetadata["Backup Name"].ToString(), writer.ToString());
    }
}