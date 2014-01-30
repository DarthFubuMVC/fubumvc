using System;
using FubuMVC.Core.Registration.Nodes;
using Owin;

namespace FubuMVC.OwinHost.Middleware
{
    public class MiddlewareNode : Node<MiddlewareNode, MiddlewareChain>, IAppBuilderConfiguration
    {
        private readonly Action<IAppBuilder> _configuration;

        private string _description = "Owin Middleware";
        private BehaviorCategory _category = BehaviorCategory.Process;

        public MiddlewareNode(Action<IAppBuilder> configuration)
        {
            _configuration = configuration;
        }

        public string Description()
        {
            return _description;
        }

        public MiddlewareNode Description(string description)
        {
            _description = description;
            return this;
        }

        public MiddlewareNode Category(BehaviorCategory category)
        {
            _category = category;
            return this;
        }

        public BehaviorCategory Category()
        {
            return _category;
        }

        public void Configure(IAppBuilder builder)
        {
            _configuration(builder);
        }
    }
}