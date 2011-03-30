using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.Tests
{
	[TestFixture]
	public class SparkRegistrationTester
	{
		[Test]
		public void configures_settings_with_default_assemblies_and_namespaces()
		{
			var registry = new FubuRegistry();
			registry.WithSparkDefaults();

			SparkSettings settings = null;
			registry.Services(x =>
			                  	{
			                  		settings = (SparkSettings) x.DefaultServiceFor<ISparkSettings>().Value;
			                  	});

			registry.BuildGraph();

			

			settings
				.UseAssemblies
				.ShouldContain(typeof(HtmlTag).Assembly.FullName);

			settings
				.UseAssemblies
				.ShouldContain(typeof(FubuPageExtensions).Assembly.FullName);

			settings
				.UseNamespaces
				.ShouldContain(typeof(FubuRegistryExtensions).Namespace);

			settings
				.UseNamespaces
				.ShouldContain(typeof(FubuPageExtensions).Namespace);

			settings
				.UseNamespaces
				.ShouldContain(typeof(HtmlTag).Namespace);
		}

		[Test]
		public void registers_spark_view_factory()
		{
			var registry = new FubuRegistry();
			registry.WithSparkDefaults();

			ISparkViewFactory factory = null;
			registry.Services(x =>
			                  	{
									factory = (ISparkViewFactory)x.DefaultServiceFor<ISparkViewFactory>().Value;
			                  	});

			registry.BuildGraph();

			factory.ShouldNotBeNull();
		}

		[Test]
		public void registers_spark_view_renderer()
		{
			var registry = new FubuRegistry();
			registry.WithSparkDefaults();

			var hasRenderer = false;
			registry.Services(x => x.Each((type, def) =>
			                              	{
			                  		       		if(type == typeof(ISparkViewRenderer<>))
			                  		       		{
			                  		       			hasRenderer = def.Type == typeof (SparkViewRenderer<>);
			                  		       		}
			                              	}));

			registry.BuildGraph();

			hasRenderer.ShouldBeTrue();
		}

		[Test]
		public void first_configuration_supercedes_second() // Packaging activator runs second so this is very important
		{
			var registry = new FubuRegistry();
			registry.Spark(spark => spark.Settings.AddNamespace("Test"));
			registry.WithSparkDefaults();

			SparkSettings settings = null;
			registry.Services(x =>
			                  	{
			                  		settings = (SparkSettings) x.DefaultServiceFor<ISparkSettings>().Value;
			                  	});

			registry.BuildGraph();

			settings
				.UseNamespaces
				.ShouldContain("Test");
		}
	}
}