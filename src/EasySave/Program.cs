using EasySave.Views;
using EasySave.ViewModels;

namespace EasySave
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var viewModel = new MainViewModel();
            
            var view = new ConsoleView(viewModel);

            view.Render();  
        }
    }
}