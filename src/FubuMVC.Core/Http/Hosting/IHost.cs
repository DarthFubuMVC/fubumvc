using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.Http.Hosting
{
    public interface IHost
    {
        IDisposable Start(
            int port, 
            Func<IDictionary<string, object>, Task> func, 
            IDictionary<string, object> properties
        );
    }
}