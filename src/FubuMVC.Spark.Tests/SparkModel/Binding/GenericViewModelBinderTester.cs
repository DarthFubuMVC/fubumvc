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
    public class GenericViewModelBinderTester : InteractionContext<GenericViewModelBinder>
    {
        private BindRequest _request;
        private ITemplate _template;
        private ViewDescriptor _descriptor;

        protected override void beforeEach()
        {
            _template = new Template("", "", "");
            _descriptor = new ViewDescriptor(_template);
            _template.Descriptor = _descriptor;

            _request = new BindRequest
            {
                Target = _template,
                ViewModelType = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz>",
                Types = typePool(),
                Logger = MockFor<ISparkLogger>()
            };
        }

        [Test]
        public void if_generic_view_model_type_exists_in_different_assemblies_nothing_is_assigned()
        {
            _request.ViewModelType = "FubuMVC.Spark.Tests.SparkModel.Binding.DuplicatedGeneric<FubuMVC.Spark.Tests.SparkModel.Binding.Bar>";
            ClassUnderTest.Bind(_request);

            _descriptor.ViewModel.ShouldBeNull();
        }

        [Test]
        public void if_view_model_type_exists_it_is_assigned_on_item()
        {
            ClassUnderTest.Bind(_request);
            _descriptor.ViewModel.ShouldEqual(typeof(Generic<Baz>));
        }

        [Test]
        public void if_view_model_type_does_not_exist_nothing_is_assigned()
        {
            _request.ViewModelType = "x.y.jazz<bar>";
            ClassUnderTest.Bind(_request);
            _descriptor.ViewModel.ShouldBeNull();
        }

        [Test]
        public void generic_parse_errors_are_logged()
        {
            _request.ViewModelType = "x.y.jazz<FubuMVC.Spark.Tests.SparkModel.Binding.Bar>";
            ClassUnderTest.Bind(_request);
            MockFor<ISparkLogger>()
                .AssertWasCalled(x => x.Log(Arg<Template>.Is.Same(_template), Arg<string>.Is.NotNull));
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
        public void it_does_not_bind_template_that_does_not_have_viewdescriptor_set()
        {
            _template.Descriptor = new NulloDescriptor();
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void it_does_not_bind_viewmodel_if_already_set()
        {
            _descriptor.ViewModel = typeof(Baz);
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_partials()
        {
            _request.Target = new Template("_partial.spark", "", "testing");
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_non_generics()
        {
            _request.ViewModelType = "System.String";
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

            var externalAssemblyDuplicatedType = generateType("namespace FubuMVC.Spark.Tests.SparkModel.Binding{public class DuplicatedGeneric<T>{}}", "FubuMVC.Spark.Tests.SparkModel.Binding.DuplicatedGeneric`1");

            pool.AddType<Bar>();
            pool.AddType<Baz>();
            pool.AddType<Generic<Baz>>();
            pool.AddType<Generic<Bar>>();
            pool.AddType<Generic<Baz, Bar>>();
            pool.AddType<DuplicatedGeneric<Bar>>();
            pool.AddSource(() => new[] { externalAssemblyDuplicatedType.Assembly, GetType().Assembly });
            pool.AddType(externalAssemblyDuplicatedType);

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

            var compiledAssembly = CodeDomProvider
                .CreateProvider("CSharp")
                .CompileAssemblyFromSource(parms, source)
                .CompiledAssembly;

            return compiledAssembly.GetType(fullName);
        }
    }

    public class DuplicatedGeneric<T> { }
}