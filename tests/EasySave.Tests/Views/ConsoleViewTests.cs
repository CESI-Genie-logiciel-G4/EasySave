using System.Text;
using EasySave.Helpers;

namespace EasySave.Tests.Views;

using EasySave.ViewModels;
using EasySave.Views;

public class ConsoleViewTests
{
    [Fact]
    public void ShouldDisplayMenuAndJobs()
    {
        
        var consoleView = new ConsoleView(new MainViewModel());
        var writer = new StringWriter();

        Console.SetOut(writer);
        Console.SetIn(new StringReader("4\ns"));
        
        consoleView.Render();
        
        Assert.Contains("Documents", writer.ToString());
    }
    
    [Fact]
    public void ShouldAddJob()
    {
        var viewModel = new MainViewModel();
        var consoleView = new ConsoleView(viewModel);
        var writer = new StringWriter();

        Console.SetOut(writer);
        Console.SetIn(new StringReader("2\nNew-Backup\n\n4\n\n"));
        
        consoleView.Render();
        
        Assert.Contains("Job New-Backup added", writer.ToString());
    }
    
    [Fact]
    public void ShouldRemoveJob()
    {
        var viewModel = new MainViewModel();
        var consoleView = new ConsoleView(viewModel);
        var writer = new StringWriter();

        Console.SetOut(writer);
        Console.SetIn(new StringReader("3\n1\n\n4\n\n"));
        
        consoleView.Render();
        
        Assert.Contains("Job N°1 removed", writer.ToString());
    }
}