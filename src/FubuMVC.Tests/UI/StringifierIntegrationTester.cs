using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuMVC.UI;
using FubuMVC.UI.Tags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class StringifierIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry =
                new FubuRegistry(
                    x => { x.StringConversions(s => { s.ForStruct<DateTime>(d => d.ToShortDateString()); }); });

            container = new Container(x => x.For<IFubuRequest>().Singleton());
            var facility = new StructureMapContainerFacility(container);

            new FubuBootstrapper(facility, registry).Bootstrap(new List<RouteBase>());

            var request = container.GetInstance<IFubuRequest>();


            address = new Address();
            request.Set(address);

            request.Get<Address>().ShouldBeTheSameAs(address);


            generator = container.GetInstance<TagGenerator<Address>>();
        }

        #endregion

        private Container container;
        private Address address;
        private TagGenerator<Address> generator;

        [Test]
        public void generating_a_display_for_a_date_time_field_should_use_the_stringifier_function_registered()
        {
            address.DateEntered = DateTime.Now;

            generator.DisplayFor(x => x.DateEntered).Text().ShouldEqual(address.DateEntered.ToShortDateString());
        }
    }
}