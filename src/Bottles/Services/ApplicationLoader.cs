using System;

namespace Bottles.Services
{
    public class ApplicationLoader<TSource, TApplication, TRuntime> : IApplicationLoader
        where TSource : IApplicationSource<TApplication, TRuntime>, new()
        where TApplication : IApplication<TRuntime>
        where TRuntime : IDisposable
    {
        public IDisposable Load()
        {
            return new TSource().BuildApplication().Bootstrap();
        }

        public override string ToString()
        {
            return new TSource().ToString();
        }
    }
}