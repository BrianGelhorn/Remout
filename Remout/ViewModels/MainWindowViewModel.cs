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

        private object? _movieSelected;
        public object? MovieSelected
        {
            get => _movieSelected;
            set
            {
                SetProperty(ref _movieSelected, value);
            }
        }

        private bool canSelectMovie = true;
        public bool CanSelectMovie
        {
            get => canSelectMovie;
            set => SetProperty(ref canSelectMovie, value);
        }

        string _currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        private IContainerExtension _containerExtension;
        private ISharedDataStore _sharedDataStore;

        public DelegateCommand<MovieBase> OnMovieMouseLeftUpCommand { get; set; }
        public DelegateCommand OnMovieMouseLeftDownCommand { get; set; }
        public DelegateCommand OnMovieMouseLeaveCommand { get; set; }

        public MainWindowViewModel(IContainerExtension containerExtension, ISharedDataStore sharedDataStore, IMouseClickService mouseClickService)
        {
            _containerExtension = containerExtension;
            _sharedDataStore = sharedDataStore;

            var imageAddButtonDir = new Uri(Path.Combine(_currentDir, "Media", "Thumbnails", "cross_image.png"));

            OnMovieMouseLeftDownCommand = new DelegateCommand(mouseClickService.OnMovieMouseButtonDown, CanExecuteOnMovieSelectedCommand);
            OnMovieMouseLeaveCommand = new DelegateCommand(mouseClickService.OnMovieMouseLeave, CanExecuteOnMovieSelectedCommand);
            OnMovieMouseLeftUpCommand = new DelegateCommand<MovieBase>((movie) => { mouseClickService.OnMovieMouseButtonUp(movie, AddorOpenMovie);}, CanExecuteOnMovieSelectedCommand);
            
            var addMovieButton = new MovieAdd(imageAddButtonDir);
            MoviesList.Add(addMovieButton);
        }
        private void SetCanSelectMovie(bool status) { CanSelectMovie = status; }

        private bool CanExecuteOnMovieSelectedCommand() { return CanSelectMovie; }

        private bool CanExecuteOnMovieSelectedCommand(MovieBase movie) { return CanSelectMovie; }

        private bool isMouseDown = false;

        private void AddorOpenMovie(MovieBase movie)
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
            if (CanSelectMovie)
            {
                SetCanSelectMovie(false);
                _sharedDataStore.CurrentMovieSelected = movie;
                var hostPopup = _containerExtension.Resolve<HostPopup>();
                hostPopup.ResizeMode = ResizeMode.CanMinimize;
                hostPopup.ShowDialog();
                MovieSelected = null;
                SetCanSelectMovie(true);
            }
        }
    }
}
