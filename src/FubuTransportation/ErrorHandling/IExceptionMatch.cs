using System;
using FubuTransportation.Runtime;

namespace FubuTransportation.ErrorHandling
{
    public interface IExceptionMatch
    {
        bool Matches(Envelope envelope, Exception ex);
    }
}