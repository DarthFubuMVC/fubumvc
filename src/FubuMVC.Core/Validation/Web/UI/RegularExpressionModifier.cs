using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
	public class RegularExpressionModifier : InputElementModifier
	{
		protected override void modify(ElementRequest request)
		{
			ForRule<RegularExpressionFieldRule>(request, rule => request.CurrentTag.Data("regex", rule.Expression.ToString()));
		}
	}
}