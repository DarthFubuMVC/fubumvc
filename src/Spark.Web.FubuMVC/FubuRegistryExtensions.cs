using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.UI;
using HtmlTags;
using Spark.Web.FubuMVC.Policies;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.ViewCreation;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC
{
	public static class FubuRegistryExtensions
	{
		public static FubuRegistry WithSparkDefaults(this FubuRegistry registry)
		{
			return registry.Spark(spark => spark.Policies.Add<ControllerSparkPolicy>());
		}

		public static FubuRegistry Spark(this FubuRegistry registry, Action<ConfigureSparkExpression> configure)
		{
			var settings = new SparkSettings()
				.AddAssembly(typeof (HtmlTag).Assembly)
				.AddAssembly(typeof(FubuPageExtensions).Assembly)
				.AddNamespace(typeof(FubuRegistryExtensions).Namespace) // Spark.Web.FubuMVC
				.AddNamespace(typeof(FubuPageExtensions).Namespace) // FubuMVC.Core.UI
				.AddNamespace(typeof(HtmlTag).Namespace); // HtmlTags

			var policies = new List<ISparkPolicy>();
			var visitors = new List<ISparkDescriptorVisitor>();

			var expression = new ConfigureSparkExpression(settings, policies, visitors);
			
			PackageRegistry
				.Packages
				.Each(package => expression.Settings.AddViewFolder("~/bin/{0}/{1}/{2}/views/".ToFormat(FubuMvcPackages.FubuPackagesFolder, 
					package.Name, FubuMvcPackages.WebContentFolder)));

			configure(expression);

			var factory = new SparkViewFactory(settings);
			var resolver = new SparkPolicyResolver(policies);
			var visitorRegistry = new SparkDescriptorVisitorRegistry(visitors);

			registry
				.Services(c =>
				{
					c.SetServiceIfNone<ISparkViewFactory>(factory);
					c.SetServiceIfNone(factory.Settings);
					c.SetServiceIfNone(typeof(ISparkViewRenderer<>), typeof(SparkViewRenderer<>));
				});

			registry
				.Views
				.Facility(new SparkViewFacility(factory, resolver))
				.TryToAttach(x => x.by(new ActionAndViewMatchedBySparkViewDescriptors(resolver, visitorRegistry)));

			return registry;
		}
	}
}