using System;
using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    public static class UriExtensions
    {
        private static HashSet<string> _localhosts = new HashSet<string>(new[] { "localhost", "127.0.0.1" }, StringComparer.OrdinalIgnoreCase);

        public static Uri NormalizeLocalhost(this Uri uri)
        {
            if (_localhosts.Contains(uri.Host))
            {
                return new UriBuilder(uri) { Host = System.Environment.MachineName }.Uri;
            }
            return uri;
        }
    }
}