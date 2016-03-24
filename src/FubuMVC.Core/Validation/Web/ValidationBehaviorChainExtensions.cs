using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Validation.Web
{
	public static class ValidationBehaviorChainExtensions
	{
		 public static ValidationNode ValidationNode(this BehaviorChain chain)
		 {
			 var node = chain.OfType<IHaveValidation>().SingleOrDefault();
             if (node != null)
             {
                 return node.Validation;
             }

		     return Web.ValidationNode.Empty();
		 }
	}
}