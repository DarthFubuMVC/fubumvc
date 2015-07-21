using System;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    [Title("Always")]
    public class Always : IExceptionMatch
    {
        public bool Matches(Envelope envelope, Exception ex)
        {
            return true;
        }

        public override string ToString()
        {
            return "Always";
        }
    }
}