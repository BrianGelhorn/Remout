using Prism.Mvvm;
using Remout.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Remout.SharedData
{
    public class SharedDataStore : ISharedDataStore
    {
        public Movie CurrentMovieSelected { get; set; }
    }
}
