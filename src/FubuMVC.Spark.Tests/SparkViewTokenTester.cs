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
        private SparkItem _item;

        [SetUp]
        public void SetUp()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _item = new SparkItem(Path.Combine(root, "Views", "Home", "Home.spark"), root, FubuSparkConstants.HostOrigin)
                        {
                            Namespace = String.Join(".", new[] {GetType().Name, "Views", "Home"}),
                            ViewModelType = typeof (ProductModel)
                        };
            _token = new SparkViewToken(_item);
        }

        [Test]
        public void name_is_item_name()
        {
            _token.Name.ShouldEqual(_item.Name());
        }

        [Test]
        public void folder_is_item_namespace()
        {
            _token.Folder.ShouldEqual(_item.Namespace);
        }

        [Test]
        public void view_model_type_is_item_view_model_type()
        {
            _token.ViewModelType.ShouldEqual(_item.ViewModelType);
        }

        [Test]
        public void the_node_is_of_sparkviewoutput_type()
        {
            _token.ToBehavioralNode().ShouldBeOfType<SparkViewOutput>();
        }

        [Test]
        public void description_should_contain_view_path()
        {
            var view = (SparkViewOutput)_token.ToBehavioralNode();
            view.Description.ShouldContain(_item.RelativePath());
        }
    }

    public class ProductModel
    {
    }
}