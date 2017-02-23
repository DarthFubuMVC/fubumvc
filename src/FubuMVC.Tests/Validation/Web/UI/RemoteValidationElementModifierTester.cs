using FubuCore;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Urls;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.Remote;
using FubuMVC.Core.Validation.Web.UI;
using HtmlTags;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class RemoteValidationElementModifierTester
    {
        private RemoteValidationElementModifier theModifier;
        private HtmlTag theTag;
        private ElementRequest theRequest;
        private RemoteRuleGraph theRemoteGraph;
        private InMemoryServiceLocator theServices;
        private IUrlRegistry theUrls;
        private RemoteFieldRule theRemoteRule;

        public RemoteValidationElementModifierTester()
        {
            theModifier = new RemoteValidationElementModifier();
            theTag = new HtmlTag("input");

            theRequest = ElementRequest.For<RemoteTarget>(x => x.Username);
            theRequest.ReplaceTag(theTag);

            theRemoteGraph = new RemoteRuleGraph();
            theRemoteRule = theRemoteGraph.RegisterRule(theRequest.Accessor, new UniqueUsernameRule());

            theUrls = new StubUrlRegistry();

            theServices = new InMemoryServiceLocator();
            theServices.Add(theRemoteGraph);
            theServices.Add(theUrls);

            theRequest.Attach(theServices);
        }

        [Fact]
        public void always_matches()
        {
            theModifier.Matches(null).ShouldBeTrue();
        }

        [Fact]
        public void registers_the_validation_def()
        {
            theModifier.Modify(theRequest);

            var def = theRequest.CurrentTag.Data("remote-rule").As<RemoteValidationDef>();
            def.url.ShouldBe(theUrls.RemoteRule());
            def.rules.ShouldHaveTheSameElementsAs(theRemoteRule.ToHash());
        }
        
        [Fact]
        public void no_registration_when_no_rules_are_found()
        {
            theRemoteGraph = new RemoteRuleGraph();
            theServices.Add(theRemoteGraph);

            theModifier.Modify(theRequest);

            theRequest.CurrentTag.Data("remote-rule").ShouldBeNull();
        }

        public class RemoteTarget
        {
            public string Username { get; set; }
        }
    }
}