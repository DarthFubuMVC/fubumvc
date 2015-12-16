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

       
        private readonly IList<IContinuationSource> _sources = new List<IContinuationSource>(); 

        public void AddContinuation(IContinuation continuation)
        {
            _sources.Add(new ContinuationSource(continuation));
        }

        public IContinuation Continuation(Envelope envelope, Exception ex)
        {
            var count = _sources.Count;
            switch (count)
            {
                case 0:
                    return Requeue;

                case 1:
                    return _sources.Single().DetermineContinuation(envelope, ex);

                default:
                    return new CompositeContinuation(_sources.Select(x => x.DetermineContinuation(envelope, ex)).ToArray());
            }
        }

        public void AddCondition(IExceptionMatch condition)
        {
            _conditions.Add(condition);
        }

        public void AddContinuation(IContinuationSource source)
        {
            _sources.Add(source);
        }

        public IList<IContinuationSource> Sources
        {
            get { return _sources; }
        }

        public IEnumerable<IExceptionMatch> Conditions
        {
            get { return _conditions; }
        }

        public IContinuation DetermineContinuation(Envelope envelope, Exception ex)
        {
            return Matches(envelope, ex) ? Continuation(envelope, ex) : null;
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

            if (_sources.Count > 1)
            {
                description.AddList("Continuations", _sources);
            }
            else if (_sources.Count == 1)
            {
                description.ShortDescription = Description.For(_sources.Single()).ShortDescription;
            }
        }
    }

    public class ContinuationSource : IContinuationSource, DescribesItself
    {
        private readonly IContinuation _continuation;

        public ContinuationSource(IContinuation continuation)
        {
            _continuation = continuation;
        }

        public IContinuation DetermineContinuation(Envelope envelope, Exception ex)
        {
            return _continuation;
        }

        public void Describe(Description description)
        {
            if (_continuation is DescribesItself)
            {
                _continuation.As<DescribesItself>().Describe(description);
            }
            else
            {
                description.ShortDescription = _continuation.ToString();
            }
        }
    }

    public interface IContinuationSource
    {
        IContinuation DetermineContinuation(Envelope envelope, Exception ex);
    }
}