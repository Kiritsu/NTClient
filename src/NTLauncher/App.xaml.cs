using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NTLauncher.ViewModels;
using NTLauncher.Views;

namespace NTLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = BuildServiceProvider();

            var mainView = services.GetRequiredService<MainWindow>();
            mainView.Show();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddTransient(x => new MainWindow { DataContext = x.GetService<MainWindowModel>() })
                .AddTransient<MainWindowModel>()
                .AddSingleton<SparkClient>()
                .BuildServiceProvider();
        }
    }
}
