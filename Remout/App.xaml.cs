﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prism.Ioc;
using Prism.Unity;
using Remout.Views;
using Remout.ViewModels;
using System.Data;
using System.Windows;
using Remout.Services;
using Remout.SharedData;

namespace Remout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {

        }

        private MainWindow mainWindow;
        protected override Window CreateShell()
        {
            mainWindow = Container.Resolve<MainWindow>();
            return mainWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISharedDataStore, SharedDataStore>();
            containerRegistry.RegisterSingleton<IUpnpService, UpnpService>();
            containerRegistry.RegisterSingleton<IServerService, ServerService>();
            containerRegistry.RegisterSingleton<IClientService, ClientService>();
        }

        protected override void OnExit(ExitEventArgs e)
        {

            base.OnExit(e);
        }
    }

}
