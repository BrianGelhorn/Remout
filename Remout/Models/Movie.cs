using Prism.Commands;
using Prism.Events;
using System.Windows.Media;

namespace Remout.Models
{
    public abstract class MovieBase
    {
        public string? Title { get; set; }
        public Uri? MovieDir { get; set; }
        public Uri? ImageDir { get; set; }
        public Stretch Stretch { get; set; }
    }

    public class Movie : MovieBase
    {

        public Movie(
            string Title, 
            Uri MovieDir,
            Uri ImageDir)
        { 
            this.Title = Title;
            this.MovieDir = MovieDir;
            this.ImageDir = ImageDir;
            Stretch = Stretch.UniformToFill;
        }
    }

    public class MovieAdd : MovieBase
    {
        public MovieAdd(Uri addButtonImageDir)
        {
            ImageDir = addButtonImageDir;
            Stretch = Stretch.Uniform;
        }
    }
}
