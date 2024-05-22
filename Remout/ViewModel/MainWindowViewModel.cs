using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remout.ViewModel
{
    class MainWindowViewModel : BindableBase
    {
        public IEnumerable<string> SampleList { get; set;} = ["Sample 1", "Sample 2", "Sample 3", "Sample 4", "Sample 5", "Sample 6"];
    }
}
