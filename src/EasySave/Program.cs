using EasySave.Views;
using EasySave.ViewModels;
using Logger.Transporters;
using EasySave.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasySave
{
    internal static class Program
    {
        private static void Main()
        {
            HistoryService.LoadHistory();
            
            var services = new ServiceCollection();
            services.AddApplicationServices();
            using var serviceProvider = services.BuildServiceProvider();
            
            var jobService = serviceProvider.GetService<JobService>();
            var localizationService = serviceProvider.GetService<LocalizationService>();

            localizationService?.SetLanguage("en");
            jobService?.LoadJobs();
            
            var view = serviceProvider.GetRequiredService<ConsoleView>();
            view.Render();
        }
    }
}