using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.Urls;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.UI;
using HtmlTags;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
	
	public class FieldEqualityFormModifierTester
	{
		private BehaviorGraph theGraph;
		private ValidationGraph theValidationGraph;

		private FieldEqualityRule rule1;
		private FieldEqualityRule rule2;

		private StringToken token1;
		private StringToken token2;

	    public FieldEqualityFormModifierTester()
	    {
			token1 = StringToken.FromKeyString("TestKeys:Key1", "Token 1");
			token2 = StringToken.FromKeyString("TestKeys:Key2", "Token 2");

			rule1 = FieldEqualityRule.For<LoFiTarget>(x => x.Value1, x => x.Value2);
			rule1.Token = token1;

			rule2 = FieldEqualityRule.For<LoFiTarget>(x => x.Value1, x => x.Value2);
			rule2.Token = token2;

			var source = new ConfiguredValidationSource(new IValidationRule[] {rule1, rule2});

			theValidationGraph = ValidationGraph.For(source);
			
			theGraph = BehaviorGraph.BuildFrom(x =>
			{
				x.Actions.IncludeType<FormValidationModeEndpoint>();
			    x.Features.Validation.Enable(true);
                x.Policies.Local.Add<ValidationPolicy>();
			});
		}

		private FormRequest requestFor<T>() where T : class, new()
		{
			var services = new InMemoryServiceLocator();
			services.Add<IChainResolver>(new ChainResolutionCache(theGraph));
			services.Add(theValidationGraph);
			services.Add<IChainUrlResolver>(new ChainUrlResolver(new OwinHttpRequest()));

			var request = new FormRequest(new ChainSearch { Type = typeof(T) }, new T());
			request.Attach(services);
			request.ReplaceTag(new FormTag("test"));

			return request;
		}

		[Fact]
		public void modifies_the_form()
		{
			var theRequest = requestFor<AjaxTarget>();

			var modifier = new FieldEqualityFormModifier();
			modifier.Modify(theRequest);

			var rawValues = theRequest
				.CurrentTag
				.Data(FieldEqualityFormModifier.FieldEquality)
				.As<IDictionary<string, object>>();

			var values = rawValues.Children("rules");
			values.ShouldHaveCount(2);
		}

		[Fact]
		public void no_strategies()
		{
			var theRequest = requestFor<AjaxTarget>();
			theRequest.Chain.ValidationNode().Clear();

			var modifier = new FieldEqualityFormModifier();
			modifier.Modify(theRequest);

			theRequest.CurrentTag.Data(FieldEqualityFormModifier.FieldEquality).ShouldBeNull();
		}
	}
}