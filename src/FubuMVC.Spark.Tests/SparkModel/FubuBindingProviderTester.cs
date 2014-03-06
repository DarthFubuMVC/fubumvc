using System;
using System.IO;
using System.Linq;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.Bindings;
using Spark.FileSystem;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class FubuBindingProviderTester : InteractionContext<FubuBindingProvider>
    {
        private ISparkTemplateRegistry _templateRegistry;
        private BindingRequest _request;
        private IViewFolder _viewFolder;

        protected override void beforeEach()
        {
            const string viewPath = "/_Package1_/Handlers/Models/SerieSL.spark";
            var appRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            var packageRoot = Path.Combine(appRoot, "App", "fubu-packages", "Package1", "WebContent");
            
            var binding1 = new SparkTemplate(Path.Combine(packageRoot, "Handlers", "Shared", "bindings.xml"), packageRoot, "Package1");
            var binding2 = new SparkTemplate(Path.Combine(appRoot, "Shared", "bindings.xml"), appRoot, ContentFolder.Application);
            var viewPathPolicy = new ViewPathPolicy<ISparkTemplate>();
            viewPathPolicy.Apply(binding1);
            viewPathPolicy.Apply(binding2);

            _viewFolder = MockFor<IViewFolder>();
            _viewFolder.Expect(x => x.GetViewSource(binding1.ViewPath)).Return(new FileSystemViewFile(binding1.FilePath));
            _viewFolder.Expect(x => x.GetViewSource(binding2.ViewPath)).Return(new FileSystemViewFile(binding2.FilePath));

            _request = new BindingRequest(_viewFolder) {ViewPath = viewPath};

            _templateRegistry = MockFor<ISparkTemplateRegistry>();
            _templateRegistry.Expect(x => x.BindingsForView(viewPath)).Return(new[]{binding1, binding2});
        }

        [Test]
        public void the_templates_used_as_binding_source_are_retrieved_using_the_sparktemplates_object()
        {
            ClassUnderTest.GetBindings(_request).ToList();
            _templateRegistry.VerifyAllExpectations();
        }

        [Test]
        public void get_bindings_gets_the_binding_view_source_using_the_view_folder()
        {
            ClassUnderTest.GetBindings(_request).ToList();
            _viewFolder.VerifyAllExpectations();
        }

        [Test]
        public void get_bindings_return_the_correct_binding_items()
        {
            var bindings = ClassUnderTest.GetBindings(_request).ToList();
            bindings.ShouldHaveCount(3);

            bindings.ElementAt(0).ElementName.ShouldEqual("hello");
            bindings.ElementAt(0).Phrases.ShouldHaveCount(1);
            bindings.ElementAt(0).Phrases.ElementAt(0).Nodes.ShouldHaveCount(1);
            bindings.ElementAt(0).Phrases.ElementAt(0).Nodes.ElementAt(0)
                .ShouldBeOfType<BindingLiteral>().Text.ShouldEqual(@"""Hi from Package1""");

            bindings.ElementAt(1).ElementName.ShouldEqual("hello");
            bindings.ElementAt(1).Phrases.ShouldHaveCount(1);
            bindings.ElementAt(1).Phrases.ElementAt(0).Nodes.ShouldHaveCount(1);
            bindings.ElementAt(1).Phrases.ElementAt(0).Nodes.ElementAt(0)
                .ShouldBeOfType<BindingLiteral>().Text.ShouldEqual(@"""Hi from Host""");

            bindings.ElementAt(2).ElementName.ShouldEqual("bye");
            bindings.ElementAt(2).Phrases.ShouldHaveCount(1);
            bindings.ElementAt(2).Phrases.ElementAt(0).Nodes.ShouldHaveCount(1);
            bindings.ElementAt(2).Phrases.ElementAt(0).Nodes.ElementAt(0)
                .ShouldBeOfType<BindingLiteral>().Text.ShouldEqual(@"""Bye from Host""");
        }
    }
}