using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.UI;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    [TestFixture]
    public class ValidationConventionTester
    {
        [Test]
        public void adds_validation_action_filter_for_lofi_endpoints()
        {
            var call = ActionCall.For<SampleInputModel>(x => x.Test(null));
            
            var chain = new BehaviorChain();
            chain.AddToEnd(call);

            ValidationPolicy.ApplyValidation(call, new ValidationSettings());

            var nodes = chain.ToArray();
			var node = nodes[0].As<IHaveValidation>();

			node.As<ActionFilter>().HandlerType.ShouldBe(typeof (ValidationActionFilter<string>));
        }

        [Test]
        public void adds_ajax_validation_action_filter_for_ajax_endpoints()
        {
            var call = ActionCall.For<SampleAjaxModel>(x => x.post_model(null));

            var chain = new BehaviorChain();
            chain.AddToEnd(call);

            ValidationPolicy.ApplyValidation(call, new ValidationSettings());

            var nodes = chain.ToArray();
        	var node = nodes[0].As<IHaveValidation>();

        	node.ShouldBeOfType<AjaxValidationNode>();
        }

		[Test]
		public void applies_modifications_from_the_settings()
		{
			var call = ActionCall.For<SampleInputModel>(x => x.Test(null));

			var chain = new BehaviorChain();
			chain.AddToEnd(call);

			var settings = new ValidationSettings();
			settings.ForInputType<string>(x =>
			{
				x.Clear();
				x.RegisterStrategy(RenderingStrategies.Inline);
			});

			ValidationPolicy.ApplyValidation(call, settings);

			chain.ValidationNode().ShouldHaveTheSameElementsAs(RenderingStrategies.Inline);
		}

		[Test]
		public void no_modifications_from_the_settings()
		{
			var call = ActionCall.For<SampleInputModel>(x => x.Test(null));

			var chain = new BehaviorChain();
			chain.AddToEnd(call);

			var settings = new ValidationSettings();
			settings.ForInputType<int>(x =>
			{
				x.Clear();
				x.RegisterStrategy(RenderingStrategies.Inline);
			});

			ValidationPolicy.ApplyValidation(call, settings);

			chain.ValidationNode().ShouldHaveTheSameElementsAs(RenderingStrategies.Summary, RenderingStrategies.Highlight);
		}
    }
}