﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Services
{
    public interface ICommunicationService
    {
        public TcpListener CreateAndStartTcpServer(int port);
        public Task ListenForConnections(TcpListener tcpServer);

    }
}
