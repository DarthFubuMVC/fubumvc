using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.UI.Forms;
using HtmlTags.Conventions;

namespace FubuMVC.Core.Validation.Web.UI
{
	public class FieldEqualityFormModifier : ITagModifier<FormRequest>
	{
		public const string FieldEquality = "field-equality";

		public bool Matches(FormRequest token)
		{
			return true;
		}

		public void Modify(FormRequest request)
		{
			var validation = request.Chain.ValidationNode();
			if (validation.IsEmpty())
			{
				return;
			}

			var graph = request.Services.GetInstance<ValidationGraph>();
			var plan = graph.PlanFor(request.Input.GetType());
			var rules = plan.FindRules<FieldEqualityRule>();

			if (!rules.Any())
			{
				return;
			}

			var data = buildValues(rules);
			request.CurrentTag.Data(FieldEquality, data);
		}

		private IDictionary<string, object> buildValues(IEnumerable<FieldEqualityRule> rules)
		{
			return new Dictionary<string, object> {{"rules", rules.Select(x => x.ToValues())}};
		}
	}
}