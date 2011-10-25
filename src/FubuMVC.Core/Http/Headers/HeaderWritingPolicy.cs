using System;
using FubuMVC.Core.Registration;
using System.Linq;
using FubuCore;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http.Headers
{
    public class HeaderWritingPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var chains = graph.Actions()
                .Where(x => x.HasOutput && x.OutputType().CanBeCastTo<IHaveHeaders>())
                .Select(x => x.ParentChain())
                .Distinct();

            chains.Each(chain =>
            {
                var action = chain.Calls.Last();
                action.AddAfter(Wrapper.For<WriteHeadersBehavior>());
            });
        }
    }
}