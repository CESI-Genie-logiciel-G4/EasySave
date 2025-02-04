using System.Text;

namespace EasySave.Tests.Views;

using EasySave.ViewModels;
using EasySave.Views;

public class ConsoleViewTests
{
    [Fact]
    public void ShouldDisplayCorrectOutput()
    {
        var consoleView = new ConsoleView(new MainViewModel());
        var writer = new StringWriter();

        Console.SetOut(writer);
        Console.SetIn(new StringReader("2"));

        consoleView.Render();
        
        Assert.Contains("Full", writer.ToString());
        Assert.Contains("Differential", writer.ToString());
    }
}