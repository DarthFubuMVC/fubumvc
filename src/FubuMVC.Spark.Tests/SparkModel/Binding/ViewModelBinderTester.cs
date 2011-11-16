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
        private ITemplate _template;
        private Parsing _parsing;
        private ViewDescriptor _descriptor;

        protected override void beforeEach()
        {
            _template = new Template("", "", "");
            _descriptor = new ViewDescriptor(_template);
            _template.Descriptor = _descriptor;

            _parsing = new Parsing
            {
                ViewModelType = "FubuMVC.Spark.Tests.SparkModel.Binding.Baz"
            };

            _request = new BindRequest
            {
                Target = _template,
                Parsing = _parsing,
                Types = typePool(),
                Logger = MockFor<ISparkLogger>()
            };
        }

        [Test]
        public void if_view_model_type_fullname_exists_in_different_assemblies_nothing_is_assigned()
        {
            _parsing.ViewModelType = typeof(Bar).FullName;
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
            _parsing.ViewModelType = "x.y.jazz";
            ClassUnderTest.Bind(_request);
            _descriptor.ViewModel.ShouldBeNull();
        }

        [Test]
        public void if_view_model_type_is_generic_nothiudoes_not_exist_nothing_is_assigned()
        {
            _parsing.ViewModelType = "x.y.jazz";
            ClassUnderTest.Bind(_request);
            _descriptor.ViewModel.ShouldBeNull();
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
        public void it_does_not_bind_generic_viewmodels()
        {
            _parsing.ViewModelType = "System.Collections.List<System.String>";
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
            pool.AddType<Generic<Baz>>();
            pool.AddType<Generic<Baz, Bar>>();

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
    public class Generic<T>
    {
    }

    public class Generic<T1, T2>
    {
        
    }
}