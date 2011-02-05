using System.Collections.Generic;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.Registration.DSL;

namespace Spark.Web.FubuMVC
{
	public class ConfigureSparkExpression
	{
		private readonly SparkSettings _settings;
		private readonly List<ISparkPolicy> _sparkPolicies;
		private readonly List<ISparkDescriptorVisitor> _visitors;

		public ConfigureSparkExpression(SparkSettings settings, List<ISparkPolicy> sparkPolicies, List<ISparkDescriptorVisitor> visitors)
		{
			_settings = settings;
			_visitors = visitors;
			_sparkPolicies = sparkPolicies;
		}

		public SparkPoliciesExpression Policies { get { return new SparkPoliciesExpression(_sparkPolicies); } }
		public ConfigureSparkSettingsExpression Settings { get { return new ConfigureSparkSettingsExpression(_settings); } }
		public SparkDescriptorVisitorsExpression Output { get { return new SparkDescriptorVisitorsExpression(_visitors); } }
	}
}