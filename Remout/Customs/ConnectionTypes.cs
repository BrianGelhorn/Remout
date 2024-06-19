using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Cbor;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Microsoft.Extensions.FileProviders;
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
                Task.Run(ListenForFile);
            }

            //TODO: Implement MD5 File Verification
            public async Task ListenForFile()
            {
                var tcpStream = GetStream();
                var buffer = new byte[2048];
                var bufferLen = await tcpStream.ReadAsync(buffer);
                var movieData = Encoding.UTF8.GetString(buffer, 0, bufferLen);
                var parsed = int.TryParse(movieData.Split(";")[0], out var movieSize);
                if (!parsed) return;
                var movieTitle = movieData.Split(";")[1];
                var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                var pathForFile = Path.Combine(currentDir, "Media", "Movies", movieTitle);
                var fileStream = File.Create(pathForFile);
                bufferLen = await tcpStream.ReadAsync(buffer);
                await fileStream.WriteAsync(buffer, 0, bufferLen);
                while (true)
                {
                    bufferLen = await tcpStream.ReadAsync(buffer);
                    if (bufferLen == 0) break;
                    await fileStream.WriteAsync(buffer, 0, bufferLen);
                }
                fileStream.Close();
                OnFileCompletelyReceived();
            }

            public event EventHandler FileCompletelyReceived;

            public void OnFileCompletelyReceived()
            {
                FileCompletelyReceived.Invoke(this, EventArgs.Empty);
            }

            public async Task SendFileAsync(string Name, FileStream file)
            {
                var tcpStream = GetStream();
                var fileInfo = Encoding.UTF8.GetBytes($"{(file.Length).ToString()};{Name}");
                await tcpStream.WriteAsync(fileInfo);
                var buffer = new byte[8192];
                while (true)
                {
                    var bufferlen = await file.ReadAsync(buffer);
                    if(bufferlen == 0) break;
                    await tcpStream.WriteAsync(buffer, 0, bufferlen);
                }
                file.Close();
                Close();
                Dispose();
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
