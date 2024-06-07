using Remout.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remout.SharedData
{
    public interface ISharedDataStore
    {
        public Movie CurrentMovieSelected { get; set; }
    }
}
