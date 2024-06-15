using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remout.Customs;
using Remout.Services;

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

        private IClientService _clientService;
        public JoinWindowViewModel(IClientService clientService)
        {
            _clientService = clientService; 
            JoinToHostWithIpCommand = new DelegateCommand(JoinToHostWithIp, CanExecuteJoinToHostWithIp);
        }

        private bool CanExecuteJoinToHostWithIp()
        {
            return true;
        }
        public async void JoinToHostWithIp()
        {
            var port = await _clientService.GetHostPort(Ip);
            if(port == -1) return;
            await _clientService.GetHostPort("181.46.193.8");
            await _clientService.ConnectToHost(Ip, port, ConnectionTypes.ConnectionType.File);
        }
    }
}
