using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Tests.Validation.Web.UI;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
	
	public class ValidationNodeModificationTester
	{
		[Fact]
		public void matches()
		{
			new ValidationNodeModification(x => true, null).Matches(new BehaviorChain()).ShouldBeTrue();
		}

		[Fact]
		public void matches_negative()
		{
			new ValidationNodeModification(x => false, null).Matches(new BehaviorChain()).ShouldBeFalse();
		}

		[Fact]
		public void modifies_the_validation_node()
		{
			var modification = new ValidationNodeModification(null, x => x.Clear());
			var node = new AjaxValidationNode(ActionCall.For<FormValidationModeEndpoint>(x => x.post_ajax(null)));
			
			var chain = new BehaviorChain();
			chain.AddToEnd(node);

			modification.Modify(chain);

			node.ShouldHaveCount(0);
		}
	}
}