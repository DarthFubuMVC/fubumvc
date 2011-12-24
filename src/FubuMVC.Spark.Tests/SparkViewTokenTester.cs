using System;
using System.IO;
using FubuMVC.Spark.Registration.Nodes;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkViewTokenTester
    {
        private SparkViewToken _token;
        private ITemplate _template;
        private ViewDescriptor _descriptor;

        [SetUp]
        public void SetUp()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _template = new Template(Path.Combine(root, "Views", "Home", "Home.spark"), root, FubuSparkConstants.HostOrigin);
            
            _descriptor = new ViewDescriptor(_template)
            {
                Namespace = String.Join(".", new[] {GetType().Name, "Views", "Home"}),
                ViewModel = typeof (ProductModel)
            };
            
            _template.Descriptor = _descriptor;

            _token = new SparkViewToken(_descriptor);
        }

        [Test]
        public void name_is_item_name()
        {
            _token.Name.ShouldEqual(_template.Name());
        }

        [Test]
        public void folder_is_descriptor_namespace()
        {
            _token.Folder.ShouldEqual(_descriptor.Namespace);
        }

        [Test]
        public void view_model_type_is_descriptor_view_model()
        {
            _token.ViewModelType.ShouldEqual(_descriptor.ViewModel);
        }

        [Test]
        public void the_node_is_of_sparkviewoutput_type()
        {
            _token.ToBehavioralNode().ShouldBeOfType<SparkViewNode>();
        }

        [Test]
        public void description_should_contain_view_path()
        {
            var view = (SparkViewNode)_token.ToBehavioralNode();
            view.Description.ShouldContain(_template.RelativePath());
        }
    }

    public class ProductModel
    {
    }
}