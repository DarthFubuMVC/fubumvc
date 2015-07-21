using System;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class ExceptionMatch : IExceptionMatch, DescribesItself
    {
        private readonly Func<Exception, bool> _filter;
        private readonly string _description;

        public ExceptionMatch(Func<Exception, bool> filter, string description)
        {
            _filter = filter;
            _description = description;
        }

        public bool Matches(Envelope envelope, Exception ex)
        {
            return _filter(ex);
        }

        public string Description
        {
            get { return _description; }
        }

        public void Describe(Description description)
        {
            description.Title = _description;
            description.ShortDescription = string.Empty;
        }

        public override string ToString()
        {
            return _description;
        }
    }
}