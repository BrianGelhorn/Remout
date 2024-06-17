using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Remout.Customs;

namespace Remout.Services
{
    public class ClientService : IClientService
    {
        public async Task<TcpClient> ConnectToHost(string ip, int port, ConnectionTypes.ConnectionType connectionType)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);
            var tcpStream = tcpClient.GetStream();
            var dataToSend = Encoding.UTF8.GetBytes(connectionType.ToString());
            await tcpStream.WriteAsync(dataToSend);
            return connectionType switch
            {
                ConnectionTypes.ConnectionType.File => new ConnectionTypes.FileConnection(tcpClient),
                ConnectionTypes.ConnectionType.Chat => new ConnectionTypes.ChatConnection(tcpClient),
                ConnectionTypes.ConnectionType.SyncMovie => new ConnectionTypes.SyncMovieConnection(tcpClient),
                ConnectionTypes.ConnectionType.CheckPort => new CheckPortConnection(tcpClient),
                _ => tcpClient
            };
        }

        public async Task<int> GetHostPort(string ip)
        {
            for (int i = 4500; i < 4600; i++)
            {
                var port = i;
                var task = Task.Run(async() =>
                {
                    using var tcpClient = new TcpClient();
                    try
                    {
                        await tcpClient.ConnectAsync(ip, port, new CancellationTokenSource(5000).Token);
                        var bufferToSend = Encoding.UTF8.GetBytes(((int)ConnectionTypes.ConnectionType.CheckPort).ToString());
                        await tcpClient.GetStream()
                            .WriteAsync(bufferToSend);
                        var buffer = new byte[128];
                        var bytesCount = await tcpClient.GetStream().ReadAsync(buffer);
                        var decodedData = Encoding.UTF8.GetString(buffer, 0, bytesCount);
                        if (decodedData == "Remout") return port;
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    return -1;
                });
                var result = task.Result;
                if (result != -1) return result;
            }
            return -1;
        }
    }
}
