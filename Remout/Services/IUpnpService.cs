using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Services
{
    public interface IUpnpService
    {
        public Task<int?> OpenPort(int port, Protocol protocol);
        public Task<string> GetPublicIp();
    }
}
