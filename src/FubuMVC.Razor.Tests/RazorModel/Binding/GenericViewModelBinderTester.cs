using System;
using System.CodeDom.Compiler;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.RazorModel.Binding
{
    [TestFixture]
    public class GenericViewModelBinderTester : InteractionContext<GenericViewModelBinder<IRazorTemplate>>
    {
        private BindRequest<IRazorTemplate> _request;
        private IRazorTemplate _template;
        private ViewDescriptor<IRazorTemplate> _descriptor;

        protected override void beforeEach()
        {
            _template = new Template("", "", "");
            _descriptor = new ViewDescriptor<IRazorTemplate>(_template);
            _template.Descriptor = _descriptor;

            _request = new BindRequest<IRazorTemplate>
            {
                Target = _template,
                Parsing = new Parsing
                {
                    ViewModelType = "FubuMVC.Razor.Tests.RazorModel.Binding.Generic<FubuMVC.Razor.Tests.RazorModel.Binding.Baz>",
                },
                Types = typePool(),
                Logger = MockFor<ITemplateLogger>()
            };
        }

        [Test]
        public void if_generic_view_model_type_exists_in_different_assemblies_nothing_is_assigned()
        {
            _request.Parsing.ViewModelType = "FubuMVC.Razor.Tests.RazorModel.Binding.DuplicatedGeneric<FubuMVC.Razor.Tests.RazorModel.Binding.Bar>";
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
            _request.Parsing.ViewModelType = "x.y.jazz<bar>";
            ClassUnderTest.Bind(_request);
            _descriptor.ViewModel.ShouldBeNull();
        }

        [Test]
        public void generic_parse_errors_are_logged()
        {
            _request.Parsing.ViewModelType = "x.y.jazz<FubuMVC.Razor.Tests.RazorModel.Binding.Bar>";
            ClassUnderTest.Bind(_request);
            MockFor<ITemplateLogger>()
                .AssertWasCalled(x => x.Log(Arg<Template>.Is.Same(_template), Arg<string>.Is.NotNull));
        }

        [Test]
        public void it_does_not_try_to_bind_names_that_are_null_or_empty()
        {
            _request.Parsing.ViewModelType = string.Empty;
            ClassUnderTest.CanBind(_request).ShouldBeFalse();

            _request.Parsing.ViewModelType = null;
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
            _request.Target = new Template("_partial.cshtml", "", "testing");
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_non_generics()
        {
            _request.Parsing.ViewModelType = "System.String";
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void it_logs_to_tracer()
        {
            ClassUnderTest.Bind(_request);
            MockFor<ITemplateLogger>()
                .AssertWasCalled(x => x.Log(Arg<Template>.Is.Same(_template), Arg<string>.Is.NotNull, Arg<object[]>.Is.NotNull));
        }

        private TypePool typePool()
        {
            var pool = new TypePool();
            pool.AddAssembly(GetType().Assembly);

            var externalAssemblyDuplicatedType = generateType("namespace FubuMVC.Razor.Tests.RazorModel.Binding{public class DuplicatedGeneric<T>{}}", "FubuMVC.Razor.Tests.RazorModel.Binding.DuplicatedGeneric`1");

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