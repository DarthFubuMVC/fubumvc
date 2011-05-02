using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    public class SharedItemLocatorTester : InteractionContext<SharedItemLocator>
    {
        private SparkItem _foundItem;
        private SparkItem _notFoundItem;
        protected override void beforeEach()
        {
            var sharedDirectoryProvider = MockFor<ISharedDirectoryProvider>();
            var fromItem = new SparkItem("", "", "");
            var sharedDirectories = new List<string>
            {
              Path.Combine("App","Views","Shared"),
              Path.Combine("App","Handlers","Shared"),
              Path.Combine("App","Controllers","Shared"),
              Path.Combine("App","Actions","Shared"),
            };

            var items = new List<SparkItem>
            {
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(0), "Baz.spark"), "", ""),
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(1), "Fooz.spark"), "", ""),
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(2), "Foo.spark"), "", ""),
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(3), "Bazz.spark"), "", ""),
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(3), "Foo.spark"), "", "")
            };

            sharedDirectoryProvider.Stub(x => x.GetDirectories(fromItem, items)).Return(sharedDirectories);
            Services.Inject(sharedDirectoryProvider);


            _foundItem = ClassUnderTest.LocateItem("Foo", fromItem, items);
            _notFoundItem = ClassUnderTest.LocateItem("Bar", fromItem, items);
        }

        [Test]
        public void if_exists_the_item_is_not_null()
        {
            _foundItem.ShouldNotBeNull();
        }

        [Test]
        public void the_located_item_name_match_the_requested_spark_name()
        {
            _foundItem.Name().ShouldEqual("Foo");
        }

        [Test]
        public void the_located_item_is_under_the_reachable_directories()
        {
            _foundItem.DirectoryPath().ShouldEqual(Path.Combine("App", "Controllers", "Shared"));
        }

        [Test]
        public void when_not_found_returns_null()
        {
            _notFoundItem.ShouldBeNull();
        }
    }
}