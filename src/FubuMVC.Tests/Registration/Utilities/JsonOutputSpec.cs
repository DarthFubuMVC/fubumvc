using System;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Tests.Registration.Utilities
{
    public class JsonOutputSpec : ChainedBehaviorSpec<RenderJsonNode>
    {
        private readonly Type _modelType;

        public JsonOutputSpec(Type modelType)
        {
            _modelType = modelType;
        }

        protected override void doSpecificCheck(RenderJsonNode node, BehaviorSpecCheck check)
        {
            if (_modelType == node.ModelType)
            {
                check.RegisterError("Wrong model type.  Should be {0} but was {1}".ToFormat(_modelType.FullName,
                                                                                            node.ModelType.FullName));
            }
        }
    }
}