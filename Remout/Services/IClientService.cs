﻿using Remout.Customs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Services
{
    public interface IClientService
    {
        public Task<TcpClient> ConnectToHost(string ip, int port, ConnectionTypes.ConnectionType connectionType);
        public Task<int> GetHostPort(string ip);
    }
}
