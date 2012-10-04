using System;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Core.UI.Testing.Integration
{
    [TestFixture]
    public class ElementConventions_with_Profiles_Tester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Import<ProfiledHtmlConventions>();
            registry.Actions.IncludeType<ProfiledEndpoint>();

            registry.AlterSettings<ViewEngines>(x => {
                x.IfTheViewMatches(v => v.Name().Contains("Profile")).SetTagProfileTo("foo");
            });

            // This wouldn't be necessary in most circumstances, but since this application
            // and registry is really built by FubuMVC.TestingHarness, we need to add the assembly
            registry.Applies.ToAssemblyContainingType<ProfiledEndpoint>();
        }

        [Test]
        public void get_profiled_display()
        {
            ProfiledViewModelDocument.Builder = doc => {
                doc.Model.Name = "Jeremy";
                return doc.DisplayFor(x => x.Name);
            };

            endpoints.Get<ProfiledEndpoint>(x => x.get_profiled_page()).ReadAsText()
                .ShouldContain("<div class=\"foo\">Jeremy</div>");
        }
    }

    public class ProfiledEndpoint
    {
        public ProfiledViewModel get_profiled_page()
        {
            return new ProfiledViewModel();
        }
    }

    public class ProfiledViewModel : ConventionTarget
    {
        
    }

    public class ProfiledViewModelDocument : FubuHtmlDocument<ProfiledViewModel>
    {
        public static Func<ProfiledViewModelDocument, HtmlTag> Builder = x => new HtmlTag("div").Text("Nothing");

        public ProfiledViewModelDocument(IServiceLocator services, IFubuRequest request) : base(services, request)
        {
            HtmlTag tag = Builder(this);
            Add(tag);
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