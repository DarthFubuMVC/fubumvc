using System;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
	public class SetLanguageSparkDescriptorVisitor : ISparkDescriptorVisitor
	{
		private readonly LanguageType _language;
		private readonly Func<ActionCall, bool> _predicate;

		public SetLanguageSparkDescriptorVisitor(LanguageType language, Func<ActionCall, bool> predicate)
		{
			_language = language;
			_predicate = predicate;
		}

		public bool AppliesTo(ActionCall call)
		{
			return _predicate(call);
		}

		public void Visit(SparkViewDescriptor matchedDescriptor, ActionCall call)
		{
			matchedDescriptor.SetLanguage(_language);
		}
	}
}