using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Remout.Customs;

namespace Remout.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly List<ConnectionTypes.FileConnection> _fileTcpClients = [];
        private readonly List<ConnectionTypes.ChatConnection> _chatTcpClients = [];
        private readonly List<ConnectionTypes.SyncMovieConnection> _syncMovieTcpClients = [];

        public TcpListener CreateAndStartTcpServer(int port)
        {
            var tcpServer = new TcpListener(IPAddress.Any, port) { ExclusiveAddressUse = true };
            tcpServer.Start();
            Task.Run(async () => { await ListenForConnections(tcpServer);});
            return tcpServer;
        }

        public void StopTcpServer()
        {

        }

        public async Task ListenForConnections(TcpListener tcpServer)
        {
            while (true)
            {
                var tcpClient = await tcpServer.AcceptTcpClientAsync();
                await ClassifyByConnectionType(tcpClient);
                await Task.Delay(50);
            }
        }

        public async Task ListenForData()
        {
            foreach (var client in _fileTcpClients)
            {
                var stream = client.GetStream();
                if (!stream.DataAvailable) continue;
            }
            foreach (var client in _chatTcpClients)
            {
                var stream = client.GetStream();
            }
            foreach (var client in _syncMovieTcpClients)
            {
                var stream = client.GetStream();
            }
        }

        private async Task ClassifyByConnectionType(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            var buffer = new byte[8];
            var bytesCount = await stream.ReadAsync(buffer);
            var parsed = int.TryParse(Encoding.UTF8.GetString(buffer), out var decodedData);
            if (!parsed) return;
            var connectionType = (ConnectionTypes.ConnectionType)decodedData;
            switch (connectionType)
            {
                case ConnectionTypes.ConnectionType.File:
                    _fileTcpClients.Add(new ConnectionTypes.FileConnection(tcpClient));
                    break;
                case ConnectionTypes.ConnectionType.Chat:
                    _chatTcpClients.Add(new ConnectionTypes.ChatConnection(tcpClient));
                    break;
                case ConnectionTypes.ConnectionType.SyncMovie:
                    _syncMovieTcpClients.Add(new ConnectionTypes.SyncMovieConnection(tcpClient));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
