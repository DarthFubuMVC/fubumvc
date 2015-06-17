using System;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;

namespace FubuTransportation.ErrorHandling
{
    public interface IErrorHandler
    {
        IContinuation DetermineContinuation(Envelope envelope, Exception ex);
    }
}