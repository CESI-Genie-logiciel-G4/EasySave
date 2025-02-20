using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EasySave.ViewModels;
using EasySave.Views.app;

namespace EasySave
{
    public class App(MainViewModel mainViewModel) : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow(mainViewModel);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}