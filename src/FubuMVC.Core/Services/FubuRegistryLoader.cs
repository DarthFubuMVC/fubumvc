using System;

namespace FubuMVC.Core.Services
{
    public class FubuRegistryLoader<T> : IApplicationLoader where T : FubuRegistry, new()
    {
        public IDisposable Load()
        {
            var registry = new T();

            return registry.ToRuntime();
        }
    }
}