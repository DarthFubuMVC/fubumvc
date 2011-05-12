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
    public class TemplateLocatorTester : InteractionContext<TemplateLocator>
    {
        private ITemplate _applicationItem;
        private ITemplate _messageItem;
        private ITemplate _lowerCasedCompanyItem;
        private ITemplate _upperCasedCompanyItem;

        private ITemplate _headerItem;

        protected override void beforeEach()
        {
            var fromItem = new Template("", "", "");
            var sharedDirectories = new List<string>
            {
              Path.Combine("App","Views","Shared"),
              Path.Combine("App","Handlers","Shared"),
              Path.Combine("App","Controllers","Shared"),
              Path.Combine("App","Actions","Shared"),
            };

            var items = new List<Template>
            {
                new Template(Path.Combine(sharedDirectories.ElementAt(0), "Company.spark"), "", ""),
                new Template(Path.Combine(sharedDirectories.ElementAt(1), "company.spark"), "", ""),
                new Template(Path.Combine(sharedDirectories.ElementAt(2), "application.spark"), "", ""),
                new Template(Path.Combine(sharedDirectories.ElementAt(3), "application.spark"), "", ""),
                new Template(Path.Combine("App","Views","Home", "header.spark"), "", ""),
            };

            var templateDirectoryProvider = MockFor<ITemplateDirectoryProvider>();
            templateDirectoryProvider.Stub(x => x.SharedPathsOf(fromItem, items)).Return(sharedDirectories);

            _applicationItem = ClassUnderTest.LocateSharedTemplates("application", fromItem, items).FirstOrDefault();
            _messageItem = ClassUnderTest.LocateSharedTemplates("message", fromItem, items).FirstOrDefault();
            _lowerCasedCompanyItem = ClassUnderTest.LocateSharedTemplates("company", fromItem, items).FirstOrDefault();
            _upperCasedCompanyItem = ClassUnderTest.LocateSharedTemplates("Company", fromItem, items).FirstOrDefault();
            _headerItem = ClassUnderTest.LocateSharedTemplates("header", fromItem, items).FirstOrDefault();
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