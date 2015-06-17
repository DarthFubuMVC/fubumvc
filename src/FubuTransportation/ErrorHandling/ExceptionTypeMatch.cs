using System;
using FubuCore.Descriptions;
using FubuTransportation.Runtime;

namespace FubuTransportation.ErrorHandling
{
    public class ExceptionTypeMatch<T> : IExceptionMatch, DescribesItself where T : Exception
    {
        public bool Matches(Envelope envelope, Exception ex)
        {
            return ex is T;
        }

        public void Describe(Description description)
        {
            description.Title = ToString();
            description.ShortDescription = string.Empty;
        }

        public override string ToString()
        {
            return "If the exception is " + typeof(T).Name;
        }
    }
}