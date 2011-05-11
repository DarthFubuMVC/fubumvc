using System;
using System.CodeDom.Compiler;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{ 
    [TestFixture]
    public class ViewModelBinderTester : InteractionContext<ViewModelBinder>
    {
        private BindRequest _request;
        private Template _template;
        private ViewDescriptor _descriptor;

        protected override void beforeEach()
        {
            _template = new Template("", "", "");
            _descriptor = new ViewDescriptor(_template);
            _template.Descriptor = _descriptor;

            _request = new BindRequest
            {
                Target = _template,
                ViewModelType = "FubuMVC.Spark.Tests.SparkModel.Binding.Baz",
                Types = typePool(),
                Logger = MockFor<ISparkLogger>()
            };
        }

        [Test]
        public void if_view_model_type_fullname_exists_in_different_assemblies_nothing_is_assigned()
        {
            _request.ViewModelType = typeof(Bar).FullName;
            ClassUnderTest.Bind(_request);

            _descriptor.ViewModel.ShouldBeNull();
        }

        [Test]
        public void if_view_model_type_exists_it_is_assigned_on_item()
        {
            ClassUnderTest.Bind(_request);
            _descriptor.ViewModel.ShouldEqual(typeof(Baz));
        }

        [Test]
        public void if_view_model_type_does_not_exist_nothing_is_assigned()
        {
            _request.ViewModelType = "x.y.jazz";
            ClassUnderTest.Bind(_request);
            _descriptor.ViewModel.ShouldBeNull();
        }

        [Test]
        public void it_does_not_try_to_bind_names_that_are_null_or_empty()
        {
            _request.ViewModelType = string.Empty;
            ClassUnderTest.CanBind(_request).ShouldBeFalse();

            _request.ViewModelType = null;
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void it_logs_to_tracer()
        {
            ClassUnderTest.Bind(_request);
            MockFor<ISparkLogger>()
                .AssertWasCalled(x => x.Log(Arg<Template>.Is.Same(_template), Arg<string>.Is.NotNull, Arg<object[]>.Is.NotNull));
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