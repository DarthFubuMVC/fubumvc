using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Services
{
    public class FubuRegistryLoader<T> : IApplicationLoader where T : FubuRegistry, new()
    {
        public IDisposable Load(Dictionary<string, string> properties)
        {
            var registry = new T();
            if (properties.ContainsKey("Mode"))
            {
                registry.Mode = properties["Mode"];
            }

            return registry.ToRuntime();
        }
    }
}