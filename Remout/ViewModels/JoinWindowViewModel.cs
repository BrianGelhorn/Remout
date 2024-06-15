using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remout.ViewModels
{
    public class JoinWindowViewModel : BindableBase
    {
        private string _ip;
        public string Ip
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public DelegateCommand JoinToHostWithIpCommand { get; set; }

        public JoinWindowViewModel()
        {
            JoinToHostWithIpCommand = new DelegateCommand(JoinToHostWithIp, CanExecuteJoinToHostWithIp);
        }

        private bool CanExecuteJoinToHostWithIp()
        {
            return true;
        }
        public void JoinToHostWithIp()
        {

        }
    }
}
