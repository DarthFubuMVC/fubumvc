using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Bugs
{
    
    public class Issue101
    {
        [Fact]
        public void should_throw_an_exception_if_the_result_is_not_unique()
        {
            var call1 = ActionCall.For<Issue101Endpoint>(x => x.get_hello());
            var call2 = ActionCall.For<Issue101Endpoint>(x => x.get_hello());

            var graph = new BehaviorGraph();
            graph.AddChain().AddToEnd(call1);
            graph.AddChain().AddToEnd(call2);

            Exception<FubuException>.ShouldBeThrownBy(() => {
                graph.ChainFor<Issue101Endpoint>(x => x.get_hello());
            });
        }
    }

    public class Issue101Endpoint
    {
        public string get_hello()
        {
            return "Hello!";
        }
    }
}