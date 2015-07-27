using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.LightningQueues
{
    public class LightningUri
    {
        public static readonly string Protocol = "lq.tcp";

        private readonly Uri _address;
        private readonly int _port;
        private readonly string _queueName;

        public LightningUri(string uriString) : this(new Uri(uriString))
        {
            
        }

        public LightningUri(Uri address)
        {
            if (address.Scheme != Protocol)
            {
                throw new ArgumentOutOfRangeException("{0} is the wrong protocol for a LightningQueue Uri.  Only {1} is accepted", address.Scheme, Protocol);
            }

            _address = address.ToMachineUri();
            _port = address.Port;

            _queueName = _address.Segments.Last();
        }

        public Uri Address
        {
            get { return _address; }
        }

        public int Port
        {
            get { return _port; }
        }

        public string QueueName
        {
            get { return _queueName; }
        }
    }

    public static class UriExtensions
    {
        private static HashSet<string> _locals = new HashSet<string>(new[]{"localhost", "127.0.0.1"}, StringComparer.OrdinalIgnoreCase);

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