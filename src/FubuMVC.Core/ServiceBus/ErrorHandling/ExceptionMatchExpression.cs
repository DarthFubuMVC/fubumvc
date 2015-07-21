using System;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class ExceptionMatchExpression
    {
        private readonly Action<IExceptionMatch> _registration;

        public ExceptionMatchExpression(Action<IExceptionMatch> registration)
        {
            _registration = registration;
        }

        public ExceptionMatchExpression Matching(Func<Exception, bool> filter, string description)
        {
            var match = new ExceptionMatch(filter, description);
            _registration(match);

            return this;
        }

        public ExceptionMatchExpression MessageContains(string text)
        {
            return Matching(ex => ex.Message.ToLowerInvariant().Contains(text.ToLowerInvariant()),
                            "Exception message contains '{0}'".ToFormat(text));
        }

        public ExceptionMatchExpression IsType<T>() where T : Exception
        {
            return Matching(t => t is T, "Exception type is " + typeof (T).FullName);
        }

        // TODO -- more options?  By namespace/assembly ????
    }
}