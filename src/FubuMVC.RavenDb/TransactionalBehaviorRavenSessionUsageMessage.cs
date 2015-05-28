using System.Threading;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Http;
using Raven.Client;

namespace FubuMVC.RavenDb
{
    public class TransactionalBehaviorRavenSessionUsageMessage : LogRecord
    {
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public int Requests { get; set; }
        public string UserName { get; set; }

        public static TransactionalBehaviorRavenSessionUsageMessage For(IDocumentSession session, IHttpRequest request)
        {
            var userName = Thread.CurrentPrincipal.IfNotNull(x => x.Identity.Name);
            var requests = session.Advanced.NumberOfRequests;
            var url = request.FullUrl();
            var method = request.HttpMethod();
            return new TransactionalBehaviorRavenSessionUsageMessage
            {
                UserName = userName,
                Requests = requests,
                Url = url,
                HttpMethod = method
            };
        }

        public override string ToString()
        {
            return string.Format("Raven Session Usage for Url: {0}, Method: {1}, Requests: {2}, UserName: {3}",
                Url, HttpMethod, Requests, UserName);
        }
    }
}