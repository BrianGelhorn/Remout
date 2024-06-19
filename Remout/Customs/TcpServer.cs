using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Customs
{
    public class TcpServer : TcpListener
    {
        public TcpServer(IPAddress localaddr, int port) : base(localaddr, port)
        {
            Start();
        }
        private readonly List<ConnectionTypes.FileConnection> _fileTcpClients= [];
        private readonly List<ConnectionTypes.ChatConnection> _chatTcpClients = [];
        private readonly List<ConnectionTypes.SyncMovieConnection> _syncMovieTcpClients = [];

        public enum DataType
        {
            Name = 10,
            Time = 11
        }

        private async Task ListenForData()
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
        private async Task<TcpClient> ClassifyByConnectionType(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            var buffer = new byte[8];
            var bytesCount = await stream.ReadAsync(buffer);
            var parsed = int.TryParse(Encoding.UTF8.GetString(buffer), out var decodedData);
            var connectionType = (ConnectionTypes.ConnectionType)decodedData;
            switch (connectionType)
            {
                case ConnectionTypes.ConnectionType.File:
                    var fileConnection = new ConnectionTypes.FileConnection(tcpClient);
                    _fileTcpClients.Add(fileConnection);
                    return fileConnection;
                case ConnectionTypes.ConnectionType.Chat:
                    var chatConnection = new ConnectionTypes.ChatConnection(tcpClient);
                    _chatTcpClients.Add(chatConnection);
                    return chatConnection;
                case ConnectionTypes.ConnectionType.SyncMovie:
                    var syncMovieConnection = new ConnectionTypes.SyncMovieConnection(tcpClient);
                    _syncMovieTcpClients.Add(syncMovieConnection);
                    return syncMovieConnection;
                case ConnectionTypes.ConnectionType.CheckPort:
                    var checkPortConnection = new CheckPortConnection(tcpClient);
                    await checkPortConnection.SendPortAnswer();
                    return checkPortConnection;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public async Task<string> AskForData(TcpClient tcpClient, DataType dataType, int bufferSize)
        {
            var tcpStream = tcpClient.GetStream();
            var bufferToSend = Encoding.UTF8.GetBytes(((int)dataType).ToString());
            await tcpStream.WriteAsync(bufferToSend);
            var bufferToReceive = new byte[bufferSize];
            var receivedLen = await tcpStream.ReadAsync(bufferToReceive);
            var decodedData = Encoding.UTF8.GetString(bufferToReceive, 0, receivedLen);
            return decodedData;
        }
        public async Task ListenForConnections()
        {
            while (true)
            {
                var tcpClient = await ClassifyByConnectionType(await AcceptTcpClientAsync());
                OnConnectionReceived(tcpClient);
                await Task.Delay(50);
            }
        }
        public void StopTcpServer()
        {
            Stop();
            Dispose();
        }

        public event EventHandler<TcpClient> ConnectionReceived;

        protected virtual void OnConnectionReceived(TcpClient tcpClient)
        {
            ConnectionReceived?.Invoke(this, tcpClient);
        }
    }
}
