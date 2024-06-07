using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Customs
{
    public class ConnectionTypes
    {
        public class FileConnection() : TcpClient
        {
            
        }

        public class ChatConnection() : TcpClient
        {

        }

        public class SyncMovieConnection() : TcpClient
        {

        }
    }
}
