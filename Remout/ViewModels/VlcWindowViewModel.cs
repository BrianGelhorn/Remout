using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using Remout.SharedData;
using Vlc.DotNet.Forms;


namespace Remout.ViewModels
{
    public class VlcWindowViewModel : BindableBase
    {
        public DelegateCommand<VlcControl> OnClosingWindowCommand { get; set; }
        public DelegateCommand<VlcControl> OnPauseButtonClickedCommand { get; set; }
        public ISharedDataStore DataStore { get; set; }
        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }
        public VlcWindowViewModel(ISharedDataStore sharedDataStore)
        {
            DataStore = sharedDataStore;
            OnClosingWindowCommand = new DelegateCommand<VlcControl>(OnClosingWindow);
            OnPauseButtonClickedCommand = new DelegateCommand<VlcControl>(PlayOrPauseMovie);
        }

        public void PlayOrPauseMovie(VlcControl control)
        {
            if(IsPaused) control.Play();
            else control.Pause();
            IsPaused = !IsPaused;
        }

        private void OnClosingWindow(VlcControl control)
        {
            control.Stop();
        }
    }
}
