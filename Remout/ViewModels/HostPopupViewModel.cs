using System.Collections.ObjectModel;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using Remout.Models;
using Remout.SharedData;
using System.Windows;
using LibVLCSharp.Shared;
using Open.Nat;
using Remout.Services;
using Vlc.DotNet.Core;
using System.Reflection;
using Remout.Views;
using Vlc.DotNet.Forms;

namespace Remout.ViewModels
{
    public class HostPopupViewModel : BindableBase
    {
        private Visibility _windowVisibility = Visibility.Visible;
        public Visibility WindowVisibility
        {
            get => _windowVisibility;
            set => SetProperty(ref _windowVisibility, value);
        }

        public Movie? Movie { get; set; }
        private string _ip = "Getting Ip...";

        public string Ip
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        private int? _port;
        public int? Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public ISharedDataStore DataStore { get; }
        private bool _canHostMovie = false;
        public bool CanHostMovie
        {
            get => _canHostMovie;
            set => SetProperty(ref _canHostMovie, value);
        }

        public ObservableCollection<string> ParticipantsList { get; set; } = [];

        public DelegateCommand OnHostButtonClickedCommand { get; set; }
        public DelegateCommand<Window> CloseWindowCommand { get; set; }
        public HostPopupViewModel(ISharedDataStore sharedDataStore, IUpnpService upnpService)
        {
            DataStore = sharedDataStore;
            Movie = sharedDataStore.CurrentMovieSelected;
            OnHostButtonClickedCommand =
                new DelegateCommand(OpenMovie, HostButtonCanExecute).ObservesProperty(() => CanHostMovie);
            CloseWindowCommand = new DelegateCommand<Window>(CloseWindow);
            Task.Run(async () =>
            {
                Ip = await upnpService.GetPublicIp();
                Port = await upnpService.OpenPort(4500, Protocol.Tcp);
                if (Ip == "" & Port == -1) return;
                CanHostMovie = true;
            });
        }

        private void CloseWindow(Window window)
        {
            window.Close();
        }

        private bool HostButtonCanExecute()
        {
            return CanHostMovie;
        }

        private void OpenMovie()
        {
            var vlcWindow = new VlcWindow();
            var vlcControl = vlcWindow.VlcControl;
            var movieData = DataStore.CurrentMovieSelected;
            vlcControl.Play(movieData.MovieDir);
            WindowVisibility = Visibility.Collapsed;
            vlcWindow.ShowDialog();
        }
    }
}
