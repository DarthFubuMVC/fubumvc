using System;

namespace FubuMVC.Core.Services
{
    public interface IApplicationSource<TApplication, TRuntime>
        where TApplication : IApplication<TRuntime>
        where TRuntime : IDisposable
    {
        TApplication BuildApplication(string directory = null);
    }
}