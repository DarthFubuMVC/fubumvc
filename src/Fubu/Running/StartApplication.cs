using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FubuCore;

namespace Fubu.Running
{
    public class StartApplication
    {
        public static readonly string WebSocketsContent = "WebsocketsRefresh.txt";

        public string ApplicationName { get; set; }
        public int PortNumber { get; set; }
        public string PhysicalPath { get; set; }

        public bool UseProductionMode { get; set; }
        public string HtmlHeadInjectedText { get; set; }

        public string AutoRefreshWebSocketsAddress
        {
            set
            {
                if (value.IsEmpty())
                {
                    HtmlHeadInjectedText = null;
                    return;
                }

                var text = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), WebSocketsContent)
                    .ReadAllText();

                HtmlHeadInjectedText = text.Replace("%WEB_SOCKET_ADDRESS%", value);
            }
        }

        public override string ToString()
        {
            if (ApplicationName.IsNotEmpty())
            {
                return string.Format("ApplicationName: {0}, PortNumber: {1}, PhysicalPath: {2}", ApplicationName, PortNumber, PhysicalPath);
                
            }

            return "{0} with port number {1}".ToFormat(PhysicalPath, PortNumber);
        }
    }
}