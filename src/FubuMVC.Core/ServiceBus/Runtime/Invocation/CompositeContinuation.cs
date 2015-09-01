using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class CompositeContinuation : IContinuation, IEnumerable<IContinuation>, DescribesItself
    {
        private readonly IList<IContinuation> _continuations = new List<IContinuation>();

        public CompositeContinuation(params IContinuation[] continuations)
        {
            _continuations.AddRange(continuations);
        }

        public void Execute(Envelope envelope, IEnvelopeContext context)
        {
            _continuations.Each(x => {
                try
                {
                    x.Execute(envelope, context);
                }
                catch (Exception e)
                {
                    context.Error(envelope.CorrelationId, "Failed trying to run continuation {0} as part of error handling".ToFormat(x), e);
                }
            });
        }

        public void Add(IContinuation child)
        {
            _continuations.Add(child);
        }


        public IEnumerator<IContinuation> GetEnumerator()
        {
            return _continuations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Describe(Description description)
        {
            description.Title = "Composite Continuation";
            description.AddList("Continuations", _continuations);
        }
    }
}