using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

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
        private CancellationTokenSource _cancellationTokenSource = new();

        public enum DataType
        {
            Name = 10,
            Time = 11,
            Guid = 12
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
        private async Task<TcpClient> ClassifyByConnectionType(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var stream = tcpClient.GetStream();
                var buffer = new byte[8];
                var bytesCount = await stream.ReadAsync(buffer, cancellationToken);
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
                        var checkPortConnection = new ConnectionTypes.CheckPortConnection(tcpClient);
                        await checkPortConnection.SendPortAnswer();
                        return checkPortConnection;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                return new TcpClient();
            }
        }

        public async Task<Guid> GetClientGuid(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            var guidString = await AskForData(tcpClient, DataType.Guid, 128);
            return Guid.Parse(guidString);
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

        public async Task SendDataAsync(TcpClient tcpClient, DataType dataType, object dataToSend)
        {
            var tcpStream = tcpClient.GetStream();
            switch (dataType)
            {
                case DataType.Name:
                {
                    var dataToSendBytes = Encoding.UTF8.GetBytes((string)dataToSend);
                    await tcpStream.WriteAsync(dataToSendBytes, 0, dataToSendBytes.Length);
                    break;
                }
                case DataType.Time:
                {
                        var dataToSendBytes = Encoding.UTF8.GetBytes((string)dataToSend);
                        await tcpStream.WriteAsync(dataToSendBytes, 0, dataToSendBytes.Length);
                        break;
                }
                case DataType.Guid:
                {
                        var dataToSendBytes = ((Guid)dataToSend).ToByteArray();
                        await tcpStream.WriteAsync(dataToSendBytes, 0, dataToSendBytes.Length);
                        break;
                }
            }
        }
        public async Task ListenForConnections()
        {
            while (true)
            {
                try
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var tcpClient = await ClassifyByConnectionType(await AcceptTcpClientAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
                    OnConnectionReceived(tcpClient);
                    await Task.Delay(50);
                }
                catch
                {
                    break;
                }
            }
        }
        public void StopTcpServer()
        {
            _cancellationTokenSource.Cancel();
            Stop();
            _cancellationTokenSource.Dispose();
            Dispose();
        }

        public event EventHandler<TcpClient> ConnectionReceived;

        protected virtual void OnConnectionReceived(TcpClient tcpClient)
        {
            ConnectionReceived?.Invoke(this, tcpClient);
        }
    }
}
