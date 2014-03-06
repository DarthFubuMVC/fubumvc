using System;
using System.IO;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkViewTokenTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _template = new SparkTemplate(Path.Combine(root, "Views", "Home", "Home.spark"), root,
                                     ContentFolder.Application);

            _template.Namespace = String.Join(".", new[] {GetType().Name, "Views", "Home"});
            _template.ViewModel = typeof (ProductModel);

            _descriptor = new SparkDescriptor(_template, new SparkViewEngine());
            _template.Descriptor = _descriptor;

            _token = new SparkViewToken(_descriptor);
        }

        #endregion

        private SparkViewToken _token;
        private ISparkTemplate _template;
        private SparkDescriptor _descriptor;

        [Test]
        public void folder_is_descriptor_namespace()
        {
            _token.Namespace.ShouldEqual(_descriptor.Template.Namespace);
        }

        [Test]
        public void name_is_item_name()
        {
            _token.Name().ShouldEqual(_template.Name());
        }

        [Test]
        public void view_model_type_is_descriptor_view_model()
        {
            _token.ViewModel.ShouldEqual(_descriptor.Template.ViewModel);
        }
    }

    public class ProductModel
    {
    }
}