using System.Threading;
using FubuCore;
using FubuCore.Logging;
using Raven.Client;

namespace FubuMVC.RavenDb.RavenDb
{
    public class DisposeRavenSessionMessage : LogRecord
    {
        public int Requests { get; set; }
        public string UserName { get; set; }

        public static DisposeRavenSessionMessage For(IDocumentSession session)
        {
            var userName = Thread.CurrentPrincipal.IfNotNull(x => x.Identity.Name);
            var requests = session.Advanced.NumberOfRequests;
            return new DisposeRavenSessionMessage
            {
                UserName = userName,
                Requests = requests
            };
        }

        public override string ToString()
        {
            return string.Format("Disposing Raven Session. Requests: {0}, UserName: {1}", Requests, UserName);
        }
    }
}