using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fleck;
using FubuMVC.Core;

namespace Fubu.Running
{
    public class BrowserDriver : IDisposable
    {
        private int _port;
        private WebSocketServer _server;
        private string _webSocketsAddress;
        private readonly IList<IWebSocketConnection> _sockets = new List<IWebSocketConnection>();

        public void StartWebSockets()
        {
            _port = PortFinder.FindPort(8181);
            _webSocketsAddress = "ws://0.0.0.0:" + _port;

            Console.WriteLine("Broadcasting auto-refresh messages via WebSockets at " + _webSocketsAddress);

            _server = new WebSocketServer(_webSocketsAddress);
            _server.Start(socket => {
                socket.OnOpen = () => _sockets.Add(socket);

                socket.OnClose = () => _sockets.Remove(socket);


            });
        }

        public int Port
        {
            get { return _port; }
        }

        public void OpenBrowserTo(string url)
        {
            Process.Start(url);
        }

        public void RefreshPage()
        {
            _sockets.Each(x => x.Send("REFRESH"));
        }

        public void Dispose()
        {
            _sockets.Clear();
            if (_server != null) _server.Dispose();
        }
    }
}