using Prism.Mvvm;
using Remout.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Remout.SharedData.SharedDataStore;

namespace Remout.SharedData
{
    public interface ISharedDataStore
    {
        public Movie CurrentMovieSelected { get; set; }
    }
}
