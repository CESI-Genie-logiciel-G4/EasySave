﻿using EasySave.Views;
using EasySave.ViewModels;
using EasySave.Services;

namespace EasySave
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var viewModel = new MainViewModel();
            var view = new ConsoleView(viewModel);
            
            LocalizationService.SetLanguage("en");
            
            view.Render();
        }
    }
}