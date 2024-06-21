using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Prism.Commands;
using Prism.Mvvm;
using Remout.Models;
using Remout.SharedData;
using System.Windows;
using LibVLCSharp.Shared;
using Open.Nat;
using Remout.Services;
using System.Reflection;
using Remout.Customs;
using Remout.Views;

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

        private int _port;
        public int Port
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
        private TcpServer tcpServer;


        public ObservableCollection<Participant?> ParticipantsList { get; set; } = [];

        public DelegateCommand OnHostButtonClickedCommand { get; set; }
        public DelegateCommand<Window> OnCancelButtonClickedCommand {get; set; }
        public DelegateCommand CloseWindowCommand { get; set; }

        private SynchronizationContext uiContext;
        public HostPopupViewModel(ISharedDataStore sharedDataStore, IUpnpService upnpService)
        {
            uiContext = SynchronizationContext.Current;
            DataStore = sharedDataStore;
            Movie = sharedDataStore.CurrentMovieSelected;
            OnHostButtonClickedCommand =
                new DelegateCommand(OpenMovie, HostButtonCanExecute).ObservesProperty(() => CanHostMovie);
            OnCancelButtonClickedCommand = new DelegateCommand<Window>(OnCancelButtonClicked);
            CloseWindowCommand = new DelegateCommand(CloseWindow);
            Task.Run(async () =>
            {
                Ip = await upnpService.GetPublicIp();
                Port = await upnpService.OpenPort(4500, Protocol.Tcp);
                if (Ip == "" & Port == -1) return;
                tcpServer = new TcpServer(IPAddress.Any, Port);
                Task.Run(tcpServer.ListenForConnections);
                tcpServer.ConnectionReceived += OnDeviceConnected;
                CanHostMovie = true;
            });
        }

        private async void OnDeviceConnected(object? sender, TcpClient tcpClient)
        {
            switch(tcpClient)
            {
                case ConnectionTypes.InitialConnection:
                {
                        var name = await tcpServer.AskForData(tcpClient, TcpServer.DataType.Name, 128);
                        var guid = Guid.NewGuid();
                        await tcpServer.SendDataAsync(tcpClient, TcpServer.DataType.Guid, guid);
                        uiContext.Send(_ => ParticipantsList.Add(new Participant(guid, name)), null);
                        break;
                }
                case ConnectionTypes.SyncMovieConnection:
                {
                    
                    break;
                }
                case ConnectionTypes.FileConnection fileConnection:
                {
                        var deviceGuid = await tcpServer.GetClientGuid(fileConnection);
                        var participant = ParticipantsList.Where(participant => participant!.Id == deviceGuid).First();
                        if(participant == null) break;
                    await fileConnection.SendFileAsync(Movie!.Title!, File.OpenRead(Movie!.MovieDir!.AbsolutePath), participant.Progress);
                    break;
                }
                default: return;
            }
        }

        private void CloseWindow()
        {
            tcpServer.StopTcpServer();
        }

        private void OnCancelButtonClicked(Window window)
        {
            tcpServer.StopTcpServer();
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
            vlcWindow.Closing += (_, _) => { WindowVisibility = Visibility.Visible;};
            vlcWindow.ShowDialog();
        }


        public class Participant : BindableBase
        {
            public readonly Guid Id;

            private string _name = "";

            public string Name
            {
                get => _name;
                set => SetProperty(ref _name, value);
            }
            private int _progressValue;

            public int ProgressValue
            {
                get => _progressValue;
                set => SetProperty(ref _progressValue, value);
            }

            public IProgress<int> Progress;

            public Participant(Guid id, string name)
            {
                Id = id;
                Name = name;
                Progress = new Progress<int>(SetProgressValue);
            }

            private void SetProgressValue(int value)
            {
                ProgressValue = value;
            }
        }
    }
}
