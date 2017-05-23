using System.Net;
using System.Net.Sockets;

namespace LightningQueues
{
    public static class PortFinder
    {
        /// <summary>
        /// Mono compatible way to dynamically get the next available port. Good enough for tests.
        /// </summary>
        public static int FindPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}