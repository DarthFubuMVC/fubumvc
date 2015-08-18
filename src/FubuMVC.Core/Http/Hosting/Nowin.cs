using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace FubuMVC.Core.Http.Hosting
{
    public class NOWIN : IHost
    {
        public IDisposable Start(int port, Func<IDictionary<string, object>, Task> func, IDictionary<string, object> properties)
        {
            var list = new List<IDictionary<string, object>>(){new Dictionary<string, object>()};
            list[0].Add("port", port.ToString());

            properties.Add("host.Addresses", list);

            var type = Type.GetType("Nowin.OwinServerFactory, Nowin");

            var initialize = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
            var create = type.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);

            initialize.Invoke(null, new object[]{properties});
            return (IDisposable) create.Invoke(null, new object[]{func, properties});
        }
    }
}