using Remout.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Services
{
    public class MouseClickService : IMouseClickService
    {
        private bool _isMouseDown = false;

        public void OnMovieMouseLeave() { _isMouseDown = false; }

        public void OnMovieMouseButtonDown() { _isMouseDown = true; }

        public void OnMovieMouseButtonUp(MovieBase movie, Action<MovieBase> functionToExecute)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                functionToExecute(movie);
            }
        }
    }
}
