using System.Diagnostics;
using System.Reflection;
using FubuMVC.Core.Registration;
using HtmlTags;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class EndpointActionSourceTester
    {
        [Test]
        public void only_finds_methods_that_follow_the_right_pattern_and_are_not_on_object()
        {
            var pool = new TypePool(Assembly.GetExecutingAssembly());
            pool.AddType(typeof(HomeEndpoint));

            var actions = new EndpointActionSource().FindActions(pool);

            var matching = actions.Where(x => x.HandlerType == typeof(HomeEndpoint)).Select(x => x.Description);
            matching.Each(x => Debug.WriteLine(x));


            matching
                .ShouldHaveTheSameElementsAs("HomeEndpoint.Index() : HtmlDocument");
        }
    }

    public class HomeEndpoint
    {
        public HtmlDocument Index()
        {
            return new HtmlDocument();
        }
    }
}