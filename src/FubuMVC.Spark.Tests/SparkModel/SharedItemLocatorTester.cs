using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class SharedItemLocatorTester : InteractionContext<SharedItemLocator>
    {
        private SparkItem _applicationItem;
        private SparkItem _messageItem;
        private SparkItem _lowerCasedCompanyItem;
        private SparkItem _upperCasedCompanyItem;

        private SparkItem _headerItem;

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
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(0), "Company.spark"), "", ""),
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(1), "company.spark"), "", ""),
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(2), "application.spark"), "", ""),
                new SparkItem(Path.Combine(sharedDirectories.ElementAt(3), "application.spark"), "", ""),
                new SparkItem(Path.Combine("App","Views","Home", "header.spark"), "", ""),
            };

            sharedDirectoryProvider.Stub(x => x.GetDirectories(fromItem, items)).Return(sharedDirectories);
            Services.Inject(sharedDirectoryProvider);

            _applicationItem = ClassUnderTest.LocateItem("application", fromItem, items);
            _messageItem = ClassUnderTest.LocateItem("message", fromItem, items);
            _lowerCasedCompanyItem = ClassUnderTest.LocateItem("company", fromItem, items);
            _upperCasedCompanyItem = ClassUnderTest.LocateItem("Company", fromItem, items);
            _headerItem = ClassUnderTest.LocateItem("header", fromItem, items);
        }

        [Test]
        public void if_exists_the_item_is_not_null()
        {
            _applicationItem.ShouldNotBeNull();
        }

        [Test]
        public void the_located_item_name_match_the_requested_spark_name()
        {
            _applicationItem.Name().ShouldEqual("application");
        }

        [Test]
        public void the_located_item_is_under_the_reachable_directories()
        {
            _applicationItem.DirectoryPath().ShouldEqual(Path.Combine("App", "Controllers", "Shared"));
        }

        [Test]
        public void when_not_found_returns_null()
        {
            _messageItem.ShouldBeNull();
        }

        [Test]
        public void items_are_localized_by_their_exact_case_sensitive_name()
        {
            _lowerCasedCompanyItem.Name().ShouldEqual("company");
            _lowerCasedCompanyItem.DirectoryPath().ShouldEqual(Path.Combine("App", "Handlers", "Shared"));

            _upperCasedCompanyItem.Name().ShouldEqual("Company");
            _upperCasedCompanyItem.DirectoryPath().ShouldEqual(Path.Combine("App", "Views", "Shared"));
        }

        [Test]
        public void if_the_item_by_name_exists_but_is_not_under_any_of_the_shared_directories_returns_null()
        {
            _headerItem.ShouldBeNull();
        }
    }
}