using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class TemplateTester
    {
        [Test]
        public void Do()
        {
            Assert.Fail("Add more tests for this little monster");
        }

        [Test]
        public void can_get_shared_view_paths_for_origin()
        {
            Assert.Fail("UT for origin ViewPath");
//            var policy = new ViewPathPolicy<ISparkTemplate>();
//            _templates.Each(policy.Apply);
//            var origin = templateAt(1).Origin; //from pak1 which has dependency on pak2
//            var pak2SharedLocation = FileSystem.Combine("_Pak2", "Home", Shared);
//            ClassUnderTest.SharedViewPathsForOrigin(origin).ShouldContain(pak2SharedLocation);
        }

        [Test]
        public void create_view_path_for_bottle_sourced_view()
        {
            Assert.Fail("Do.");
        }

        [Test]
        public void determine_namespace_at_the_root_of_the_assembly()
        {
            Assert.Fail("Do.");
        }

        [Test]
        public void determine_namespace_when_you_are_not_at_the_root_of_the_assembly()
        {
            Assert.Fail("Do.");
        }

        [Test]
        public void binding_to_view_models()
        {
            Assert.Fail("Do.");

            /*
             * 1.) success
             * 2.) ignore if partial
             * 3.) ignore if view model type is empty
             * 4.) ignore if 
             */
        }


        /*
    [TestFixture]
    public class GenericViewModelBinderTester : InteractionContext<GenericViewModelBinder<RazorTemplate>>
    {
        private BindRequest<RazorTemplate> _request;
        private RazorTemplate _template;
        private ViewDescriptor<RazorTemplate> _descriptor;

        protected override void beforeEach()
        {
            _template = new RazorTemplate("", "", "");
            _descriptor = new ViewDescriptor<RazorTemplate>(_template);
            _template.Descriptor = _descriptor;

            _request = new BindRequest<RazorTemplate>
            {
                Target = _template,
                Types = typePool(),
                Logger = MockFor<ITemplateLogger>()
            };
        }

        [Test]
        public void if_generic_view_model_type_exists_in_different_assemblies_nothing_is_assigned()
        {
            ClassUnderTest.Bind(_request);

            _descriptor.Template.ViewModel.ShouldBeNull();
        }

        [Test]
        public void if_view_model_type_exists_it_is_assigned_on_item()
        {
            ClassUnderTest.Bind(_request);
            _descriptor.Template.ViewModel.ShouldEqual(typeof(Generic<Baz>));
        }

        [Test]
        public void if_view_model_type_does_not_exist_nothing_is_assigned()
        {
            ClassUnderTest.Bind(_request);
            _descriptor.Template.ViewModel.ShouldBeNull();
        }

        [Test]
        public void generic_parse_errors_are_logged()
        {
            ClassUnderTest.Bind(_request);
            MockFor<ITemplateLogger>()
                .AssertWasCalled(x => x.Log(Arg<RazorTemplate>.Is.Same(_template), Arg<string>.Is.NotNull));
        }

        [Test]
        public void it_does_not_try_to_bind_names_that_are_null_or_empty()
        {
            ClassUnderTest.CanBind(_request).ShouldBeFalse();

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
        public void does_not_bind_non_generics()
        {
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
         */
    }
}