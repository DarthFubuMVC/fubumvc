using System;

namespace FubuMVC.Core.Services
{
    public interface IApplicationLoader
    {
        IDisposable Load();
    }
}