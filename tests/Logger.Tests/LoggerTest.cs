using Logger.LogEntries;
using Logger.Transporters;

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
    public void CopyLogTest()
    {
        const string expectedOperation = "Copy";
        const string expectedBackupName = "save1";
        const string expectedSourcePath = @"\\UNC\source\Path";
        const string expectedDestinationPath = @"UNC\destination\Path";
        const long expectedFileSize = 12;
        const double expectedTransferTime = 3.356;
        
        var logDirectory = "/home/florent/Documents/Work at CESI/Génie logiciel/Projet/";
        var consoleTransporter = new FileJsonTransporter(logDirectory);
        
        var logger = Logger.GetInstance();
        logger.SetupTransporters([consoleTransporter]);
        
        CopyFileLogEntry copyLog = new CopyFileLogEntry(expectedBackupName, 
                                                        expectedSourcePath, 
                                                        expectedDestinationPath, 
                                                        expectedFileSize, 
                                                        expectedTransferTime);

        
        logger.Log(copyLog);
    }
}