using System;
using FubuMVC.Core.Registration;
using FubuCore;
using FubuTestingSupport;
using FubuMVC.Spark.SparkModel;
using NUnit.Framework;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    [TestFixture]
	public class FullTypeNameStrategyTester : InteractionContext<FullTypeNameStrategy>
	{
		private string _typeName;
		
		protected override void beforeEach()
		{
			_typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Baz";		    					
			Services.Inject<TypePool>(typePool());
		}

    	[Test]
		public void if_types_with_fullname_exist_in_different_assemblies_nothing_is_resolved()
		{			
			ClassUnderTest.ResolveType(typeof(Bar).FullName).ShouldBeNull();
		}

    	[Test]
		public void if_type_with_fullname_exists_it_is_resolved()
		{
			ClassUnderTest.ResolveType(_typeName).ShouldEqual(typeof(Baz));
		}

    	[Test]
		public void if_type_with_fullname_does_not_exist_nothing_is_resolved()
		{
			ClassUnderTest.ResolveType("x.y.jazz").ShouldBeNull();
		}

		private TypePool typePool ()
		{
			var pool = new TypePool (GetType().Assembly);
			
			pool.AddType (generateType ("namespace FubuMVC.Spark.Tests.SparkModel.Binding{public class Bar{}}", "FubuMVC.Spark.Tests.SparkModel.Binding.Bar"));
			pool.AddType<Bar>();
			pool.AddType<Baz>();
			
			return pool;
		}
		
		private static Type generateType (string source, string fullName)
		{
			var parms = new CompilerParameters
			{
            	GenerateExecutable = false,
            	GenerateInMemory = true,
            	IncludeDebugInformation = false
			};

			return CodeDomProvider
				.CreateProvider ("CSharp")
				.CompileAssemblyFromSource (parms, source)
				.CompiledAssembly
				.GetType (fullName);
		} 
	}
	
	public class Bar {}
	public class Baz {}
}
