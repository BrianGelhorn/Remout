using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Services
{
    public class CommunicationService : ICommunicationService
    {
        public TcpListener CreateAndStartTcpServer(int port)
        {
            var tcpServer = new TcpListener(IPAddress.Any, port) { ExclusiveAddressUse = true };
            tcpServer.Start();
            return tcpServer;
        }

        public async Task ListenForConnections(TcpListener tcpServer)
        {
            while (true)
            {
                var tcpClient = await tcpServer.AcceptTcpClientAsync();
                await ClasifyByConnectionType(tcpClient);
                await Task.Delay(50);
            }
        }

        public async Task ClasifyByConnectionType(TcpClient tcpClient)
        {

        }
    }
}
