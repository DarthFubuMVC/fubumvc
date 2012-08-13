using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Chrome
{
    [TestFixture]
    public class ChromeBehaviorIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ChromedEndpoints>();
        }

        [Test]
        public void can_fetch_endpoints_that_are_chromed()
        {
            endpoints.Get<ChromedEndpoints>(x => x.get_text1())
                .ReadAsText().ShouldEqual("**text1**");

            endpoints.Get<ChromedEndpoints>(x => x.get_text2())
                .ReadAsText().ShouldEqual("~~text2~~");
        }
    }

    public class ChromedEndpoints
    {
        public string First(FirstChrome content)
        {
            return "**" + content.InnerContent + "**";
        }

        public string Second(SecondChrome content)
        {
            return "~~" + content.InnerContent + "~~";
        }

        [Chrome(typeof(FirstChrome))]
        public string get_text1()
        {
            return "text1";
        }

        [Chrome(typeof(SecondChrome))]
        public string get_text2()
        {
            return "text2";
        }
    }

    public class FirstChrome : ChromeContent{}
    public class SecondChrome : ChromeContent{}
}