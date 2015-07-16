using System;

namespace Bottles.Services
{
    public interface IApplicationSource<TApplication, TRuntime>
        where TApplication : IApplication<TRuntime>
        where TRuntime : IDisposable
    {
        TApplication BuildApplication();
    }
}