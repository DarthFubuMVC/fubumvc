using FubuMVC.TestingHarness;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Core.UI.Testing.Integration
{
    [TestFixture]
    public class ElementConventions_with_Profiles_Tester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            
        }
    }

    public class ProfiledHtmlConventions : HtmlConventionRegistry
    {
        public ProfiledHtmlConventions()
        {
            Profile("foo", profile => {
                profile.Displays.Always.BuildBy(request => {
                    return new HtmlTag("div").Text(request.StringValue()).AddClass("foo");
                });
            });
        }
    }

    
}