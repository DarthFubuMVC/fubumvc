using System;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public interface IExceptionMatch
    {
        bool Matches(Envelope envelope, Exception ex);
    }
}