using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;
using FubuMVC.Tests.UI;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class BindingContextTester
    {
        private InMemoryRequestData request;
        private IServiceLocator locator;
        private BindingContext context;

        [SetUp]
        public void SetUp()
        {
            request = new InMemoryRequestData();
            locator = MockRepository.GenerateMock<IServiceLocator>();

            context = new BindingContext(request, locator);
        }

        [Test]
        public void get_regular_property()
        {
            request["Address1"] = "2035 Ozark";
            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            string address1 = string.Empty;
            context.Value(property, o => address1 = (string) o);

            address1.ShouldEqual(request["Address1"]);
        }

        [Test]
        public void get_property_that_falls_through_to_the_second_naming_strategy()
        {
            request["User-Agent"] = "hank";
            var property = ReflectionHelper.GetProperty<State>(x => x.User_Agent);

            string agent = null;
            context.Value(property, o => agent = (string) o);

            agent.ShouldEqual("hank");
        }

        [Test]
        public void prefix_with_returns_a_working_binding_context()
        {
            request["AddressAddress1"] = "479 SW 85th St";
            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            string address1 = string.Empty;
            context.PrefixWith("Address").Value(property, o => address1 = (string)o);

            address1.ShouldEqual(request["AddressAddress1"]);
        }

        public class State
        {
            public string User_Agent { get; set; }
        }
    }

    
}