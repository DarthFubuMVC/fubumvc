using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Tests.Registration.Utilities
{
    public class JsonOutputSpec : ChainedBehaviorSpec<ConnegOutputNode>
    {
        private readonly Type _modelType;

        public JsonOutputSpec(Type modelType)
        {
            _modelType = modelType;
        }

        protected override void doSpecificCheck(ConnegOutputNode node, BehaviorSpecCheck check)
        {
            if (_modelType == node.InputType)
            {
                check.RegisterError("Wrong model type.  Should be {0} but was {1}".ToFormat(_modelType.FullName,
                                                                                            node.InputType.FullName));
            }

            if (typeof(JsonFormatter) != node.SelectedFormatterTypes.SingleOrDefault())
            {
                check.RegisterError("Not a json output conneg node");
            }
        }
    }
}