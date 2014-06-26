using System.Diagnostics;
using System.Reflection;
using FubuMVC.Core.Registration;
using HtmlTags;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class EndpointActionSourceTester
    {
        [Test]
        public void only_finds_methods_that_follow_the_right_pattern_and_are_not_on_object()
        {
            var actions = new EndpointActionSource().As<IActionSource>().FindActions(Assembly.GetExecutingAssembly());

            var matching = actions.Where(x => x.HandlerType == typeof(HomeEndpoint)).Select(x => x.Description);
            matching
                .ShouldHaveTheSameElementsAs("HomeEndpoint.Index() : HtmlDocument");
        }

        [Test]
        public void finds_methods_case_insensitively()
        {
            var actions = new EndpointActionSource().As<IActionSource>().FindActions(Assembly.GetExecutingAssembly());

            var matching = actions.Where(x => x.HandlerType == typeof(WeirdEndPoint)).Select(x => x.Description);

            matching
                .ShouldHaveTheSameElementsAs("WeirdEndPoint.get_something() : String");
        }
    }

    public class HomeEndpoint
    {
        public HtmlDocument Index()
        {
            return new HtmlDocument();
        }
    }

    public class WeirdEndPoint
    {
        public string get_something()
        {
            return "something";
        }
    }
}