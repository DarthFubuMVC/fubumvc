using System;

namespace Bottles.Services
{
    public interface IApplication<T> where T : IDisposable
    {
        T Bootstrap();
    }
}