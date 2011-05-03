using System;
using FubuMVC.Core.Registration;
using FubuCore;
using FubuTestingSupport;
using FubuMVC.Spark.SparkModel;
using NUnit.Framework;
using System.CodeDom;
using System.CodeDom.Compiler;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
	[TestFixture]
	public class ViewModelBinderTester : InteractionContext<ViewModelBinder>
	{
		private SparkItem _item;
		private BindContext _context;
		
		protected override void beforeEach ()
		{
			_item = new SparkItem("filePath", "rootPath", "origin");
			_context = new BindContext()
			{
				ViewModelType = GetType().FullName,
				Tracer = MockFor<ISparkPackageTracer>()
			};
		}
		
		[Test]
		public void it_does_not_try_to_bind_names_that_are_null_or_empty()
		{
			_context.ViewModelType = string.Empty;
			ClassUnderTest.CanBind(_item, _context).ShouldBeFalse();
			_context.ViewModelType = null;
			ClassUnderTest.CanBind(_item, _context).ShouldBeFalse();
		}
		
		[Test]
		public void it_delegates_to_type_resolver()
		{
			ClassUnderTest.Bind(_item, _context);
			MockFor<ITypeResolver>().AssertWasCalled(x => x.ResolveType(GetType().FullName));
		}
		
		[Test]
		public void it_logs_to_tracer()
		{
			ClassUnderTest.Bind(_item, _context);
			MockFor<ISparkPackageTracer>().AssertWasCalled(x => x.Trace(Arg<SparkItem>.Is.Same(_item), Arg<string>.Is.NotNull, Arg<object[]>.Is.NotNull));
		}
	}
}