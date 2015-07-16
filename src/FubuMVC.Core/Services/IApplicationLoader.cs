using System;

namespace Bottles.Services
{
    public interface IApplicationLoader
    {
        IDisposable Load();
    }
}