using System.Diagnostics;
using Avalonia;
using EasySave.Helpers;
using EasySave.ViewModels;
using EasySave.Services;
using EasySave.Views.Console;

namespace EasySave
{
    public enum UiType
    {
        Gui,
        Console
    }

    public static class Program
    {
        private static readonly MainViewModel MainViewModel = new();
        
        private static async Task Main(string[] args)
        {
            var mode = args.Length == 0 ? "gui" : args[0];
            var valid = Enum.TryParse(mode, true, out UiType uiType);
            
            if (!valid)
            {
                Console.WriteLine(LocalizationService.GetString("InvalidUiType"), uiType);
                DisplayUsage();
                return;
            }
            
            JobService.LoadJobs();
            ExtensionService.LoadEncryptedExtensions();
            HistoryService.LoadHistory();
            LocalizationService.SetLanguage("en");
            
            switch (uiType)
            {
                case UiType.Gui:
                    BuildAvaloniaApp(MainViewModel).StartWithClassicDesktopLifetime([]);
                    break;
                case UiType.Console:
                    var view = new ConsoleView(MainViewModel);
                    await view.Render();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        
        private static void DisplayUsage()
        {
            var executableName = Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName);

            Console.WriteLine(LocalizationService.GetString("\nUsage"));
            Console.WriteLine($"\t{executableName} [Mode]");
            Console.WriteLine();

            Console.WriteLine(LocalizationService.GetString("AvailableModes"));
            foreach (var type in Enum.GetValues<UiType>())
            {
                Console.WriteLine($"\t{type}  - {LocalizationService.GetString($"UiType_{type}")}");
            }
        }

        private static AppBuilder BuildAvaloniaApp(MainViewModel mainViewModel)
            => AppBuilder.Configure(() => new App(mainViewModel))
                .UsePlatformDetect()
                .LogToTrace();
    }
}