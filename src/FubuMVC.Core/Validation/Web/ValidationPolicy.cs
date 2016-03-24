using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Validation.Web
{
    public class ValidationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<ValidationSettings>();
            var filter = settings.As<IApplyValidationFilter>();

            graph
                .Actions()
                .Where(call => filter.Filter(call.ParentChain()))
                .Each(call => ApplyValidation(call, settings));
        }

        public static void ApplyValidation(ActionCall call, ValidationSettings settings)
        {
            BehaviorNode node;
            if(call.ResourceType().CanBeCastTo<AjaxContinuation>())
            {
                node = new AjaxValidationNode(call);
            }
            else
            {
                var builder = typeof (LoFiValidationNodeBuilder<>).CloseAndBuildAs<IValidationNodeBuilder>(call.InputType());
                node = builder.BuildNode();
            }

			call.AddBefore(node);
			settings.Modify(call.ParentChain());
        }

        public interface IValidationNodeBuilder
        {
            BehaviorNode BuildNode();
        }

        public class LoFiValidationNodeBuilder<T> : IValidationNodeBuilder where T : class
        {
            public BehaviorNode BuildNode()
            {
				return ValidationActionFilter.ValidationFor<ValidationActionFilter<T>>(x => x.Validate(null));
            }
        }
    }
}