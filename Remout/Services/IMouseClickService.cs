using Remout.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Services
{
    interface IMouseClickService
    {
        void OnMovieMouseLeave();
        void OnMovieMouseButtonDown();
        void OnMovieMouseButtonUp(MovieBase movie, Action<MovieBase> functionToExecute);
    }
}
