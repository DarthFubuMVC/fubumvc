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
    public class ViewModelBinderTester : InteractionContext<ViewModelBinder<IRazorTemplate>>
    {
        private BindRequest<IRazorTemplate> _request;
        private IRazorTemplate _template;
        private ViewDescriptor<IRazorTemplate> _descriptor;

        protected override void beforeEach()
        {
            _template = new RazorTemplate("", "", "");
            _descriptor = new ViewDescriptor<IRazorTemplate>(_template);
            _template.Descriptor = _descriptor;

            _request = new BindRequest<IRazorTemplate>
            {
                Target = _template,
                Parsing = new Parsing
                {
                    ViewModelType = "FubuMVC.Razor.Tests.RazorModel.Binding.Baz",
                },
                Types = typePool(),
                Logger = MockFor<ITemplateLogger>()
            };
        }

        [Test]
        public void if_view_model_type_fullname_exists_in_different_assemblies_nothing_is_assigned()
        {
            _request.Parsing.ViewModelType = typeof(Bar).FullName;
            ClassUnderTest.Bind(_request);

            _descriptor.Template.ViewModel.ShouldBeNull();
        }

        [Test]
        public void if_view_model_type_exists_it_is_assigned_on_item()
        {
            ClassUnderTest.Bind(_request);
            _descriptor.Template.ViewModel.ShouldEqual(typeof(Baz));
        }

        [Test]
        public void if_view_model_type_does_not_exist_nothing_is_assigned()
        {
            _request.Parsing.ViewModelType = "x.y.jazz";
            ClassUnderTest.Bind(_request);
            _descriptor.Template.ViewModel.ShouldBeNull();
        }

        [Test]
        public void if_view_model_type_is_generic_nothiudoes_not_exist_nothing_is_assigned()
        {
            _request.Parsing.ViewModelType = "x.y.jazz";
            ClassUnderTest.Bind(_request);
            _descriptor.Template.ViewModel.ShouldBeNull();
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
            _descriptor.Template.ViewModel = typeof(Baz);
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_partials()
        {
            _request.Target = new RazorTemplate("_partial.cshtml", "", "testing");
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void it_does_not_bind_generic_viewmodels()
        {
            _request.Parsing.ViewModelType = "System.Collections.List<System.String>";
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void it_logs_to_tracer()
        {
            ClassUnderTest.Bind(_request);
            MockFor<ITemplateLogger>()
                .AssertWasCalled(x => x.Log(Arg<RazorTemplate>.Is.Same(_template), Arg<string>.Is.NotNull, Arg<object[]>.Is.NotNull));
        }


        private TypePool typePool()
        {
            var pool = new TypePool();
            pool.AddAssembly(GetType().Assembly);

            pool.AddType(generateType("namespace FubuMVC.Razor.Tests.RazorModel.Binding{public class Bar{}}", "FubuMVC.Razor.Tests.RazorModel.Binding.Bar"));
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