using System;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using FubuMVC.Spark.SparkModel;
using NUnit.Framework;
using System.CodeDom.Compiler;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{ 
    [TestFixture]
    public class ViewModelBinderTester : InteractionContext<ViewModelBinder>
    {
        private BindContext _context;
        private SparkItem _sparkItem;

        protected override void beforeEach()
        {
            _sparkItem = new SparkItem("", "", "");
            _context = new BindContext
            {
                ViewModelType = "FubuMVC.Spark.Tests.SparkModel.Binding.Baz",
                TypePool = typePool(),
                Tracer = MockFor<ISparkTracer>()
            };
        }

        [Test]
        public void if_view_model_type_fullname_exists_in_different_assemblies_nothing_is_assigned()
        {
            _context.ViewModelType = typeof(Bar).FullName;
            ClassUnderTest.Bind(_sparkItem, _context);
            _sparkItem.ViewModelType.ShouldBeNull();
        }

        [Test]
        public void if_view_model_type_exists_it_is_assigned_on_item()
        {
            ClassUnderTest.Bind(_sparkItem, _context);
            _sparkItem.ViewModelType.ShouldEqual(typeof(Baz));
        }

        [Test]
        public void if_view_model_type_does_not_exist_nothing_is_assigned()
        {
            _context.ViewModelType = "x.y.jazz";
            ClassUnderTest.Bind(_sparkItem, _context);
            _sparkItem.ViewModelType.ShouldBeNull();
        }

        [Test]
        public void it_does_not_try_to_bind_names_that_are_null_or_empty()
        {
            _context.ViewModelType = string.Empty;
            ClassUnderTest.CanBind(_sparkItem, _context).ShouldBeFalse();
            _context.ViewModelType = null;
            ClassUnderTest.CanBind(_sparkItem, _context).ShouldBeFalse();
        }

        [Test]
        public void it_logs_to_tracer()
        {
            ClassUnderTest.Bind(_sparkItem, _context);
            MockFor<ISparkTracer>().AssertWasCalled(x => x.Trace(Arg<SparkItem>.Is.Same(_sparkItem), Arg<string>.Is.NotNull, Arg<object[]>.Is.NotNull));
        }


        private TypePool typePool()
        {
            var pool = new TypePool(GetType().Assembly);

            pool.AddType(generateType("namespace FubuMVC.Spark.Tests.SparkModel.Binding{public class Bar{}}", "FubuMVC.Spark.Tests.SparkModel.Binding.Bar"));
            pool.AddType<Bar>();
            pool.AddType<Baz>();

            return pool;
        }
        private static Type generateType(string source, string fullName)
        {
            var parms = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false
            };

            return CodeDomProvider
                .CreateProvider("CSharp")
                .CompileAssemblyFromSource(parms, source)
                .CompiledAssembly
                .GetType(fullName);
        } 
    }

    public class Bar { }
    public class Baz { }
}