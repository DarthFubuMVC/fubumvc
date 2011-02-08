using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration.DSL
{
	public class SparkDescriptorVisitorsExpression
	{
		private readonly IList<ISparkDescriptorVisitor> _visitors;

		public SparkDescriptorVisitorsExpression(IList<ISparkDescriptorVisitor> visitors)
		{
			_visitors = visitors;
		}

		public SparkDescriptorVisitorsExpression ToJavaScriptWhen(Func<ActionCall, bool> predicate)
		{
			return ToLanguageWhen(LanguageType.Javascript, predicate);
		}

		public SparkDescriptorVisitorsExpression ToLanguageWhen(LanguageType language, Func<ActionCall, bool> predicate)
		{
			_visitors.Add(new SetLanguageSparkDescriptorVisitor(language, predicate));
			return this;
		}
	}
}