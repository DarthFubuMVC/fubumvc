using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Configuration;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class ExceptionHandlerNode : BehaviorNode, DescribesItself
    {
        private readonly HandlerChain _chain;

        public ExceptionHandlerNode(HandlerChain chain)
        {
            _chain = chain;
        }

        public new HandlerChain Chain
        {
            get { return _chain; }
        }

        protected override IConfiguredInstance buildInstance()
        {
            var instance = new SmartInstance<ExceptionHandlerBehavior>();
            instance.Ctor<HandlerChain>().Is(_chain);

            return instance;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Wrapper; }
        }

        public void Describe(Description description)
        {
            description.Title = "Error Handling";
            description.AddList("Policies", _chain.ErrorHandlers);
        }
    }
}