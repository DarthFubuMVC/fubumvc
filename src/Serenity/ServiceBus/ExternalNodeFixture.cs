using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using StoryTeller;

namespace Serenity.ServiceBus
{
    public abstract class ExternalNodeFixture : Fixture
    {
        protected ExternalNode AddTestNode<T>(string name) where T : FubuRegistry
        {
            var graph = Context.Service<ChannelGraph>();
            var node = new ExternalNode(name, typeof(T), graph);
            node = TestNodes.AddNode(name, node);
            return node;
        }
    }
}