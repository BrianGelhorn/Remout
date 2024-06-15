using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Remout.Customs;

namespace Remout.Services
{
    public class ClientService : IClientService
    {
        private readonly TcpClient tcpClient;

        public ClientService()
        {
            tcpClient = new TcpClient();
        }
        public async Task ConnectToHost(string ip, int port, ConnectionTypes.ConnectionType connectionType)
        {
            if (tcpClient.Connected) return;
            await tcpClient.ConnectAsync(ip, port);
            var tcpStream = tcpClient.GetStream();
            var dataToSend = Encoding.UTF8.GetBytes(connectionType.ToString());
            await tcpStream.WriteAsync(dataToSend);
        }

        public async Task<int> GetHostPort(string ip)
        {
            for (int i = 4500; i < 4600; i++)
            {
                var port = i;
                await Task.Run(async() =>
                {
                    using var tcp = new TcpClient();
                    try
                    {
                        await tcp.ConnectAsync(ip, port);
                        await tcp.GetStream()
                            .WriteAsync(Encoding.UTF8.GetBytes(ConnectionTypes.ConnectionType.CheckPort.ToString()));
                        var buffer = new byte[16];
                        var bytesCount = await tcp.GetStream().ReadAsync(buffer);
                        if (Encoding.UTF8.GetString(buffer) == "Remout") return port;
                    }
                    catch { }

                    return -1;
                });
                await Task.Delay(50);
            }
            return -1;
        }
    }
}
