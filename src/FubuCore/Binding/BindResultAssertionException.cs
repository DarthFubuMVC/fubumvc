using System;
using System.Collections.Generic;
using System.Text;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public class BindResultAssertionException : ApplicationException
    {
        private readonly Type _type;
        private readonly IList<ConvertProblem> _problems;

        public BindResultAssertionException(Type type, IList<ConvertProblem> problems)
        {
            _type = type;
            _problems = problems;
        }

        public override string Message
        {
            get 
            {
                var builder = new StringBuilder();
                builder.AppendFormat("Failure while trying to bind object of type '{0}'", _type.FullName);
                
                _problems.Each(p =>
                {
                    builder.AppendFormat("Property: {0}, Value: '{1}', Exception:{2}{3}{2}",
                                         p.PropertyName(), p.Value, Environment.NewLine, p.ExceptionText);
                });

                return builder.ToString();
            } 
        }

        public IList<ConvertProblem> Problems { get { return _problems; } }
        public Type Type { get { return _type; } }
    }
}