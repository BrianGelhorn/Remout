using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Cbor;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using Remout.Models;

namespace Remout.Customs
{
    public class ConnectionTypes
    {
        public enum ConnectionType
        {
            File = 0,
            Chat =1,
            SyncMovie = 2,
            CheckPort = 3
        }

        static Type GetConnectionType(ConnectionType connectionType)
        {
            return connectionType switch
            {
                ConnectionType.File => typeof(FileConnection),
                ConnectionType.Chat => typeof(ChatConnection),
                ConnectionType.SyncMovie => typeof(SyncMovieConnection),
                _ => throw new Exception("You gotta set the connection type value")
            };
        }

        public class FileConnection() : TcpClient
        {
            public FileConnection(TcpClient tcpClient) : this()
            {
                Client = tcpClient.Client;
            }

            public async Task StartListeningForFile()
            {
                var tcpStream = GetStream();
                while (true)
                {
                    var buffer = new byte[4096];
                    await tcpStream.ReadExactlyAsync(buffer);
                    var decodedData = Encoding.UTF8.GetString(buffer);
                    await Task.Delay(50);
                }
            }
        }

        public class ChatConnection() : TcpClient
        {
            public ChatConnection(TcpClient tcpClient) : this()
            {
                Client = tcpClient.Client;
            }
        }

        public class SyncMovieConnection() : TcpClient
        {

            public SyncMovieConnection(TcpClient tcpClient) : this()
            {
                Client = tcpClient.Client;
                Task.Run(ListenForData);
            }

            public event EventHandler<TcpServer.DataType> AskedForData;

            protected virtual void OnAskedForData(TcpServer.DataType dataType)
            {
                AskedForData.Invoke(this, dataType);
            }

            public async Task SendData(string data)
            {
                await GetStream().WriteAsync(Encoding.UTF8.GetBytes(data));
            }

            private async Task ListenForData()
            {
                var tcpStream = GetStream();
                while (true)
                {
                    if (tcpStream.DataAvailable)
                    {
                        var buffer = new byte[128];
                        await tcpStream.ReadExactlyAsync(buffer);
                        var decodedData = Encoding.UTF8.GetString(buffer);
                        OnAskedForData((TcpServer.DataType)(int.Parse(decodedData)));
                    }
                    await Task.Delay(50);
                }
            }

            public async Task<bool> SendMovieData(Movie movie)
            {
                var stream = GetStream();
                var dataToSend = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(movie));
                try
                {
                    await stream.WriteAsync(dataToSend);
                }
                catch(Exception e)
                {
                    Debug.WriteLine($"{e.GetType()}: {e.Message}");
                    return false;
                }
                return true;
            }

            public async Task GetMovieData()
            {

            }
        }
    }

    public class CheckPortConnection() : TcpClient
    {
        public CheckPortConnection(TcpClient tcpClient) : this()
        {
            Client = tcpClient.Client;
        }

        public async Task SendPortAnswer()
        {
            await GetStream().WriteAsync("Remout"u8.ToArray());
        }
    }
}
