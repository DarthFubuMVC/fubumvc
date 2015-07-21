using System;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
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