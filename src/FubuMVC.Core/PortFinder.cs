using System;
using System.Net;
using System.Net.Sockets;
using FubuCore;

namespace FubuMVC.Core
{
    public static class PortFinder
    {
        public static int FindPort(int start)
        {
            for (int i = start; i < start + 50; i++)
            {
                if (tryPort(i)) return i;
            }

            throw new InvalidOperationException("Could not find a port to bind to");
        }

        private static bool tryPort(int port)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            var endpoint = new IPEndPoint(IPAddress.Any, port);

            try
            {
                socket.Bind(endpoint);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                socket.SafeDispose();
            }

        }
    }
}