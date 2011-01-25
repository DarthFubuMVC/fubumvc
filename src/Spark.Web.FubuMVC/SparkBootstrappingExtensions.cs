using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.UI.Tags;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.ViewCreation;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC
{
	public static class SparkBootstrappingExtensions
	{
		public static FubuApplication Spark(this FubuApplication application, Action<ConfigureSparkExpression> configure)
		{
			application.ModifyRegistry(registry =>
			                           	{
			                           		var settings = new SparkSettings()
			                           			.AddAssembly(typeof (PartialTagFactory).Assembly)
			                           			.AddNamespace("Spark.Web.FubuMVC")
			                           			.AddNamespace("FubuMVC.Core.UI")
			                           			.AddNamespace("HtmlTags");

			                           		var policies = new List<ISparkPolicy>();
			                           		var visitors = new List<ISparkDescriptorVisitor>();

			                           		var expression = new ConfigureSparkExpression(settings, policies, visitors);
			                           		configure(expression);

			                           		var factory = new SparkViewFactory(settings);
			                           		var resolver = new SparkPolicyResolver(policies);
			                           		var visitorRegistry = new SparkDescriptorVisitorRegistry(visitors);

											registry
			                           			.Services(c =>
			                           		         	{
			                           		         		c.AddService<ISparkViewFactory>(factory);
															c.AddService(factory.Settings);
			                           		         		c.SetServiceIfNone(typeof (ISparkViewRenderer<>), typeof (SparkViewRenderer<>));
			                           		         	});

											registry
			                           			.Views
			                           			.Facility(new SparkViewFacility(factory, resolver))
			                           			.TryToAttach(x => x.by(new ActionAndViewMatchedBySparkViewDescriptors(resolver, visitorRegistry)));

			                           	});
			return application;
		}
	}
}