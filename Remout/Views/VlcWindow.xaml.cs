using System.IO;
using System.Reflection;
using System.Windows;
using Vlc.DotNet.Forms;

namespace Remout.Views
{
    /// <summary>
    /// Interaction logic for VlcWindow.xaml
    /// </summary>
    public partial class VlcWindow : Window
    {
        public VlcWindow()
        {
            InitializeComponent();
        }

        private void OnVlcLibDirectoryNeeded(object? sender, VlcLibDirectoryNeededEventArgs e)
        {
            e.VlcLibDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Vlc", "win-x64"));
        }
    }
}
