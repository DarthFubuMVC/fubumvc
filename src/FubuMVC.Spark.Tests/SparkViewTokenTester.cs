using System;
using System.IO;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.Tests.FubuMVC.Spark.Tests.Views.Home;
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

            _template.ViewModel = typeof (ProductModel);

            _token = new SparkViewToken(_template, new SparkViewEngine());

        }

        #endregion

        private ISparkTemplate _template;
        private SparkViewToken _token;

        [Test]
        public void folder_is_descriptor_namespace()
        {
            _token.Namespace.ShouldEqual(_token.Template.Namespace);
        }

        [Test]
        public void name_is_item_name()
        {
            _token.Name().ShouldEqual(_template.Name());
        }

        [Test]
        public void view_model_type_is_descriptor_view_model()
        {
            _token.ViewModel.ShouldEqual(_token.Template.ViewModel);
        }
    }

    namespace FubuMVC.Spark.Tests.Views.Home
    {
        public class ProductModel
        {
        }
    }

}