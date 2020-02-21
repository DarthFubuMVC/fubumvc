using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
            if (uri.UriContainsNonLocalIpAddress())
                return uri;

            return new UriBuilder(uri) { Host = Environment.MachineName }.Uri;
        }

        /// <summary>
        /// Detects if a Uri has a Host segment that is a non-loopback IP address.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <returns>True when the Uri contains a Host segment as an IPv4 Address which is not loopback, otherwise False.</returns>
        public static bool UriContainsNonLocalIpAddress(this Uri uri)
        {
            return (UriHostNameType.IPv4 == uri.HostNameType && !uri.Host.Equals("127.0.0.1"));
        }
    }
}
