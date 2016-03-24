using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Validation.Web
{
    public class AjaxValidationNode : Wrapper, IHaveValidation
    {
        public AjaxValidationNode(ActionCall call)
            : base(typeof(AjaxValidationBehavior<>).MakeGenericType(call.InputType()))
        {
	        InputType = call.InputType();
        	Validation = ValidationNode.Default();
        }

		public Type InputType { get; private set; }
        public ValidationNode Validation { get; private set; }
    }
}