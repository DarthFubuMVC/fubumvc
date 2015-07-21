using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class ErrorHandler : IErrorHandler, IExceptionMatch, DescribesItself
    {
        public static readonly RequeueContinuation Requeue = new RequeueContinuation();

        private readonly IList<IExceptionMatch> _conditions = new List<IExceptionMatch>(); 

       
        private readonly IList<IContinuation> _continuations = new List<IContinuation>(); 

        public void AddContinuation(IContinuation continuation)
        {
            _continuations.Add(continuation);
        }

        public IContinuation Continuation()
        {
            var count = _continuations.Count;
            switch (count)
            {
                case 0:
                    return Requeue;

                case 1:
                    return _continuations.Single();

                default:
                    return new CompositeContinuation(_continuations.ToArray());
            }
        }

        public void AddCondition(IExceptionMatch condition)
        {
            _conditions.Add(condition);
        }

        public IEnumerable<IExceptionMatch> Conditions
        {
            get { return _conditions; }
        }

        public IContinuation DetermineContinuation(Envelope envelope, Exception ex)
        {
            return Matches(envelope, ex) ? Continuation() : null;
        }

        public bool Matches(Envelope envelope, Exception ex)
        {
            if (!_conditions.Any()) return true;

            return _conditions.All(x => x.Matches(envelope, ex));
        }

        public void Describe(Description description)
        {
            description.Title = _conditions.Any() 
                ? _conditions.Select(x => Description.For(x).Title).Join(" and ") 
                : "Always";

            var continuation = Continuation();

            if (continuation is CompositeContinuation)
            {
                description.AddList("Continuations", continuation.As<CompositeContinuation>());
            }
            else
            {
                description.ShortDescription = Description.For(continuation).ShortDescription;
            }

            
        }
    }
}