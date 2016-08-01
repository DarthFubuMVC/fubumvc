using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.LightningQueues
{
    public class LightningUri
    {
        public static readonly string Protocol = "lq.tcp";

        public LightningUri(string uriString) : this(new Uri(uriString))
        {
            
        }

        public LightningUri(Uri address)
        {
            if (address.Scheme != Protocol)
            {
                throw new ArgumentOutOfRangeException(
                    $"{address.Scheme} is the wrong protocol for a LightningQueue Uri.  Only {Protocol} is accepted");
            }

            Address = address.ToMachineUri();
            Port = address.Port;

            QueueName = Address.Segments.Last();
        }

        public Uri Address { get; }

        public int Port { get; }

        public string QueueName { get; }
    }

    public static class UriExtensions
    {
        private static readonly HashSet<string> _locals = new HashSet<string>(new[]{"localhost", "127.0.0.1"}, StringComparer.OrdinalIgnoreCase);

        public static LightningUri ToLightningUri(this Uri uri)
        {
            return new LightningUri(uri);
        }

        public static LightningUri ToLightningUri(this string uri)
        {
            return new LightningUri(uri);
        }

        public static Uri ToMachineUri(this Uri uri)
        {
            if (_locals.Contains(uri.Host))
            {
                return uri.ToLocalUri();
            }
            return uri;
        }

        public static Uri ToLocalUri(this Uri uri)
        {
            return new UriBuilder(uri) { Host = Environment.MachineName }.Uri;
        }
    }
}