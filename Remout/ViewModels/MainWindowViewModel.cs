using FFMpegCore;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Remout.Models;
using Remout.Services;
using Remout.SharedData;
using Remout.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Remout.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<MovieBase> MoviesList { get; set;} = [];
        private bool isSampleListEmpty = true;

        string _currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        private DelegateCommand<object> _onMovieClickedCommand;
        public DelegateCommand<object> OnMovieClickedCommand
        {
            get => _onMovieClickedCommand;
            set => SetProperty(ref _onMovieClickedCommand, value);
        }
        private DelegateCommand _joinToHostCommand;
        public DelegateCommand JoinToHostCommand
        {
            get => _joinToHostCommand;
            set => SetProperty(ref _joinToHostCommand, value);
        }
        private bool canSelectMovie = true;
        private bool canJoinHost = true;
        private IContainerExtension _containerExtension;
        private ISharedDataStore _sharedDataStore;

        public MainWindowViewModel(IContainerExtension containerExtension, ISharedDataStore sharedDataStore)
        {
            _containerExtension = containerExtension;
            _sharedDataStore = sharedDataStore;
            var imageAddButtonDir = new Uri(Path.Combine(_currentDir, "Media", "Thumbnails", "cross_image.png"));
            var addMovieButton = new MovieAdd(imageAddButtonDir);
            OnMovieClickedCommand = new DelegateCommand<object>(AddorOpenMovie, CanExecuteAddOrOpenMovie);
            JoinToHostCommand = new DelegateCommand(OpenJoinWindow, CanJoinHost);
            MoviesList.Add(addMovieButton);
        }

        private void OpenJoinWindow()
        {
            var joinHostWindow = _containerExtension.Resolve<JoinWindow>();
            joinHostWindow.ShowDialog();
        }

        private bool CanJoinHost()
        {
            return canJoinHost;
        }

        private void SetCanSelectMovie(bool value)
        {
            canSelectMovie = value;
        }

        private bool CanExecuteAddOrOpenMovie(object movie)
        {
            return canSelectMovie;
        }

        private void AddorOpenMovie(object movie)
        {
            if(movie.GetType() == typeof(MovieAdd))
            {
                var openMovieDialog = new OpenFileDialog();
                var isAccepted = openMovieDialog.ShowDialog();

                //Return if cancel was pressed
                if(isAccepted == false) return;

                var thumbnailsDir = Path.Combine(_currentDir, "Media", "Thumbnails", Guid.NewGuid().ToString());
                var movieThumbnailDir = thumbnailsDir + ".png";
                if (!Path.Exists(movieThumbnailDir))
                {
                    FFMpeg.Snapshot(openMovieDialog.FileName, thumbnailsDir);
                }
                var movieToAdd = new Movie(
                    openMovieDialog.SafeFileName,
                    new Uri(openMovieDialog.FileName),
                    new Uri(thumbnailsDir + ".png"));

                MoviesList.Insert(0, movieToAdd);
            }
            if(movie.GetType() == typeof(Movie))
            {
                OpenMovieSelectDialog((Movie)movie);
            }
        }

        private void OpenMovieSelectDialog(Movie movie)
        {
            if (canSelectMovie)
            {
                SetCanSelectMovie(false);
                _sharedDataStore.CurrentMovieSelected = movie;
                var hostPopup = _containerExtension.Resolve<HostPopup>();
                hostPopup.ResizeMode = ResizeMode.CanMinimize;
                hostPopup.ShowDialog();
                SetCanSelectMovie(true);
            }
        }
    }
}
