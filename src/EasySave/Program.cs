using EasySave.Views;
using EasySave.ViewModels;
using Logger.Transporters;
using L = Logger.Logger;
using EasySave.Services;
using Logger.LogEntries;

namespace EasySave
{
    internal static class Program
    {
        private static void Main()
        {
            JobService.LoadJobs();
            HistoryService.LoadHistory();
            
            var logger = L.GetInstance();
            logger.SetupTransporters([
                new FileXmlTransporter(".easysave/logs/"),
                new FileJsonTransporter(".easysave/logs/"),
                new ConsoleTransporter(),
            ]);
            
            logger.Log(new GlobalLogEntry("test","mesage"));
            
            var viewModel = new MainViewModel();
            var view = new ConsoleView(viewModel);
            
            LocalizationService.SetLanguage("en");
            
            view.Render();
        }
    }
}