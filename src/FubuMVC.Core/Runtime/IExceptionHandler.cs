using System;

namespace FubuMVC.Core.Runtime
{
    public interface IExceptionHandler
    {
        bool ShouldHandle(Exception exception);
        void Handle(Exception exception);
    }
}