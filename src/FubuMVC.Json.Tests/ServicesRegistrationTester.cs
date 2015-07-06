using FubuJson;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Json.Tests
{
	public class ServicesRegistrationTester
	{
		private ServiceGraph services;

		[SetUp]
		public void SetUp()
		{
			var registry = new FubuRegistry();
			registry.Services<JsonServiceRegistry>();

			services = BehaviorGraph.BuildFrom(registry).Services;
		}

		[Test]
		public void NewtonSoftJsonSerializer_is_registered()
		{
			services.DefaultServiceFor(typeof(IJsonSerializer))
				.Type.ShouldEqual(typeof(NewtonSoftJsonSerializer));
		} 


	}
}