using EasySave.Helpers;
using EasySave.Services;
using EasySave.ViewModels;
using EasySave.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EasySave;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ConsoleView>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ConsoleHelper>();
        
        services.AddSingleton<JobService>();
        services.AddSingleton<LocalizationService>();
        services.AddSingleton(Logger.Logger.GetInstance());
        return services;
    }
}