using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.Registration.DSL;

namespace Spark.Web.FubuMVC.Tests
{
	[TestFixture]
	public class ConfigureSparkExpressionTester
	{
		private SparkSettings _settings;
		private List<ISparkPolicy> _policies;
		private List<ISparkDescriptorVisitor> _visitors;
		private ConfigureSparkExpression _expression;

		[SetUp]
		public void SetUp()
		{
			_settings = new SparkSettings();
			_policies = new List<ISparkPolicy>();
			_visitors = new List<ISparkDescriptorVisitor>();

			_expression = new ConfigureSparkExpression(_settings, _policies, _visitors);
		}

		[Test]
		public void add_view_folder()
		{
			_expression
				.Settings
				.AddViewFolder("Test");

			_settings
				.ViewFolders
				.FirstOrDefault(f => f.Parameters.ContainsKey(ConfigureSparkSettingsExpression.ViewFolderParam) && f.Parameters[ConfigureSparkSettingsExpression.ViewFolderParam] == "Test")
				.ShouldNotBeNull();
		}

		[Test]
		public void add_duplicate_view_folder()
		{
			_expression
				.Settings
				.AddViewFolder("Test")
				.AddViewFolder("Test");

			_settings
				.ViewFolders
				.Where(
					f =>
					f.Parameters.ContainsKey(ConfigureSparkSettingsExpression.ViewFolderParam) &&
					f.Parameters[ConfigureSparkSettingsExpression.ViewFolderParam] == "Test")
				.ShouldHaveCount(1);
		}
	}
}