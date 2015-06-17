using System;
using FubuCore.Descriptions;
using FubuTransportation.Runtime;

namespace FubuTransportation.ErrorHandling
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