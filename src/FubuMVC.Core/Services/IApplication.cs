using System;

namespace FubuMVC.Core.Services
{
    public interface IApplication<T> where T : IDisposable
    {
        T Bootstrap();
    }
}