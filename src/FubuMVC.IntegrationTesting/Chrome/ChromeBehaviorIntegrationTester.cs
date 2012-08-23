using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

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
            //endpoints.Get<ChromedEndpoints>(x => x.get_text1())
            //    .ReadAsText().ShouldEqual("**text1**");

            var html2 = endpoints.Get<ChromedEndpoints>(x => x.get_text2())
                .ReadAsText();
            html2.ShouldContain("~~text2~~");

            // Title
            html2.ShouldContain("<title>Some Title</title>");
        }
    }

    public class ChromedEndpoints
    {
        public string First(FirstChrome content)
        {
            return "**" + content.InnerContent + "**" ;
        }

        public string Second(SecondChrome content)
        {
            return "~~" + content.InnerContent + "~~" + "<title>{0}</title>".ToFormat(content.Title);
        }

        [Chrome(typeof(FirstChrome))]
        public string get_text1()
        {
            return "text1";
        }

        [Chrome(typeof(SecondChrome), Title = "Some Title")]
        public string get_text2()
        {
            return "text2";
        }
    }

    public class FirstChrome : ChromeContent{}
    public class SecondChrome : ChromeContent{}
}