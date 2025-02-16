using Avalonia;
using EasySave.ViewModels;
using EasySave.Services;
using EasySave.Views.Console;

namespace EasySave
{
    internal static class Program
    {
        private static void Main()
        {
            JobService.LoadJobs();
            ExtensionService.LoadEncryptedExtensions();
            HistoryService.LoadHistory();
            LocalizationService.SetLanguage("en");
            
            var ui = "gui";
            
            switch (ui)
            {
                case "console":
                    var viewModel = new MainViewModel();
                    var view = new ConsoleView(viewModel);
                    view.Render();
                    break;
                case "gui":
                    BuildAvaloniaApp().StartWithClassicDesktopLifetime([]);
                    break;
                default:
                    throw new Exception("Invalid UI");
            }
            
        }

        private static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}