using System;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Behaviors.Chrome
{
    [TestFixture]
    public class ChromeAttributeTester
    {
        [Test]
        public void attribute_whines_if_you_give_it_the_wrong_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new ChromeAttribute(GetType());
            });
        }

        [Test]
        public void applies_the_chrome_node()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<ChromedEnpoint>();
            });

            var chain = graph.BehaviorFor<ChromedEnpoint>(x => x.get_stuff());
        
            chain.OfType<ChromeNode>().Any().ShouldBeTrue();
        }

        [Test]
        public void applies_the_Title_to_the_chrome_node()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<ChromedEnpoint>();
            });

            var chain = graph.BehaviorFor<ChromedEnpoint>(x => x.get_stuff());

            chain.OfType<ChromeNode>().Single().Title().ShouldEqual("Some Title");
        }
    }

    public class ChromedEnpoint
    {
        [Chrome(typeof(DifferentChrome), Title = "Some Title")]
        public string get_stuff()
        {
            return "";
        }
    }

    public class DifferentChrome : ChromeContent{}
}