﻿using EasySave.Views;
using EasySave.ViewModels;
using Logger.Transporters;
using L = Logger.Logger;
using EasySave.Services;

namespace EasySave
{
    internal static class Program
    {
        private static void Main()
        {
            string path = ".";
            var logger = L.GetInstance();
            logger.SetupTransporters([
                new FileJsonTransporter(path),
            ]);
            
            var viewModel = new MainViewModel();
            var view = new ConsoleView(viewModel);
            
            LocalizationService.SetLanguage("en");
            
            view.Render();
        }
    }
}