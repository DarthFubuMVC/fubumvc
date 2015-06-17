using FubuTransportation.Configuration;
using StoryTeller;

namespace FubuTransportation.Serenity
{
    public abstract class ExternalNodeFixture : Fixture
    {
        protected ExternalNode AddTestNode<T>(string name) where T : FubuTransportRegistry
        {
            var graph = Context.Service<ChannelGraph>();
            var node = new ExternalNode(name, typeof(T), graph);
            node = TestNodes.AddNode(name, node);
            return node;
        }
    }
}