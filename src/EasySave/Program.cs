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

            var viewModel = new MainViewModel();
            var ui = "gui";
            switch (ui)
            {
                case "console":
                    var view = new ConsoleView(viewModel);
                    view.Render();
                    break;
                case "gui":
                    BuildAvaloniaApp(viewModel).StartWithClassicDesktopLifetime([]);
                    break;
                default:
                    throw new Exception("Invalid UI");
            }
            
        }

        private static AppBuilder BuildAvaloniaApp(MainViewModel mainViewModel)
            => AppBuilder.Configure(() => new App(mainViewModel))
                .UsePlatformDetect()
                .LogToTrace();
    }
}