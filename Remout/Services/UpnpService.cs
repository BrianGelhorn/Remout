using Microsoft.VisualBasic;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Remout.Services
{
    public class UpnpService : IUpnpService
    {
        private const string PortDescription = "Remout Port";
        NatDiscoverer discoverer = new();
        NatDevice? device;

        private async Task<bool> GetNatDevice()
        {
            bool isNatDeviceAvailable = false;
            try
            {
                device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource(5000));
                isNatDeviceAvailable = true;
            }
            catch(Exception e)
            {
                if(e.InnerException is NatDeviceNotFoundException)
                {
                    Debug.WriteLine("Nat Device Not Found. Upnp may be disabled");
                }
            }
            return isNatDeviceAvailable;
        }

        private async Task<IEnumerable<Mapping>?> CheckAllOpenPorts()
        {
            //TODO: Handle device null
            if (device == null) if (!await GetNatDevice()) return null;
            var mapping = await device!.GetAllMappingsAsync();
            return mapping;
        }

        public async Task<int> OpenPort(int port, Protocol protocol)
        {
            //TODO: Handle device null
            if (device == null) if (!await GetNatDevice()) return -1;
            var openPorts = await CheckAllOpenPorts();
            if (openPorts != null)
            {
                foreach (var mapping in openPorts)
                {
                    if (mapping.Description == PortDescription) return mapping.PublicPort;
                }
            }
            while (await device!.GetSpecificMappingAsync(protocol, port) != null)
            {
                //Increase port by 1 if the actually Upnp port is being used
                port++;
            }

            while (true)
            {
                try
                {
                    var portToCreate = new Mapping(protocol, port, port, PortDescription);
                    await device!.CreatePortMapAsync(portToCreate);
                    break;
                }
                catch
                {
                    // ignored
                }
            }
            return port;
        }

        public async Task<string> GetPublicIp()
        {
            //TODO: Handle device null
            if (device == null) if (!await GetNatDevice()) return "";
            var publicIp = (await device!.GetExternalIPAsync()).ToString();
            return publicIp;
        }
    }
}
