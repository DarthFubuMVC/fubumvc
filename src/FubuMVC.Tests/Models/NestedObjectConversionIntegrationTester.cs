using System;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class NestedObjectConversionIntegrationTester
    {
        private InMemoryRequestData data;
        private IObjectResolver resolver;

        [SetUp]
        public void SetUp()
        {
            resolver = StructureMapContainerFacility.GetBasicFubuContainer().GetInstance<IObjectResolver>();
            data = new InMemoryRequestData();
            
        }

        private T resolve<T>()
        {
            var result = resolver.BindModel(typeof(T), data);
            result.AssertNoProblems(typeof(T));

            return (T) result.Value;
        }

        [Test]
        public void get_all_valid_data_2_levels_deep()
        {
            data["Name"] = "site 1";
            data["AddressAddress1"] = "2035 Ozark";
            data["AddressCity"] = "Austin";

            var site = resolve<Site>();

            site.Name.ShouldEqual("site 1");
            site.Address.ShouldNotBeNull();
            site.Address.Address1.ShouldEqual("2035 Ozark");
            site.Address.City.ShouldEqual("Austin");
        }

        [Test]
        public void get_all_valid_data_3_levels_deep()
        {
            data["Order"] = "3";
            data["UserName"] = "Max";
            data["SiteName"] = "site 1";
            data["SiteAddressAddress1"] = "2035 Ozark";
            data["SiteAddressCity"] = "Austin";

            var request = resolve<UpdateSiteRequest>();

            request.UserName.ShouldEqual("Max");
            request.Order.ShouldEqual(3);
            request.Site.Name.ShouldEqual("site 1");
            request.Site.Address.ShouldNotBeNull();
            request.Site.Address.Address1.ShouldEqual("2035 Ozark");
            request.Site.Address.City.ShouldEqual("Austin");
        }

        [Test]
        public void get_3_deep_errors()
        {
            data["Order"] = "3";
            data["UserName"] = "Max";
            data["SiteName"] = "site 1";
            data["SiteAddressAddress1"] = "2035 Ozark";
            data["SiteAddressCity"] = "Austin";
            data["SiteAddressOrder"] = "abc";

            var result = resolver.BindModel(typeof(UpdateSiteRequest), data);
            var problem = result.Problems.Single();

            problem.PropertyName().ShouldEqual("Site.Address.Order");
        }

        public class UpdateSiteRequest
        {
            public string UserName { get; set; }
            public int Order { get; set; }
            public Site Site { get; set; }
        }

        public class Site
        {
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        public class Address
        {
            public Address()
            {
                StateOrProvince = string.Empty;
                Country = string.Empty;
                AddressType = string.Empty;
            }

            public int Order { get; set; }

            public bool IsActive { get; set; }

            [FakeRequired, FakeMaximumStringLength(100)]
            public string AddressType { get; set; }

            [FakeRequired, FakeMaximumStringLength(250)]
            public string Address1 { get; set; }

            public string Address2 { get; set; }

            [FakeRequired, FakeMaximumStringLength(250)]
            public string City { get; set; }

            [FakeRequired]
            public string StateOrProvince { get; set; }

            [FakeRequired, FakeMaximumStringLength(100)]
            public string Country { get; set; }

            [FakeRequired, FakeMaximumStringLength(50)]
            public string PostalCode { get; set; }

        }
    }
}