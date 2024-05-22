using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remout.View;
using Remout.ViewModel;
using System.Data;
using System.Windows;

namespace Remout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }
        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((services, config) =>
            {
                config.AddTransient<MainWindow>();
                config.AddTransient<MainWindowViewModel>();
            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupWindow = AppHost.Services.GetRequiredService<MainWindow>();
            startupWindow.Show();
            base.OnStartup(e);
        }
    }

}
