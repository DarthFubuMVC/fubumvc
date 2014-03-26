using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Model;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Model
{
    [TestFixture]
    public class DiagnosticGroupTester
    {
        private DiagnosticGroup theGroup = new DiagnosticGroup(Assembly.GetExecutingAssembly(), new ActionCall[0]);

        [Test]
        public void should_pick_up_the_url_from_the_FubuDiagnosticsConfiguration_class_in_that_assembly()
        {
            theGroup.Url.ShouldEqual("fake");
        }

        [Test]
        public void should_pick_up_the_title_from_the_configuration_type_in_the_assembly()
        {
            theGroup.Title.ShouldEqual(new FubuDiagnosticsConfiguration().Title);
        }

        [Test]
        public void should_pick_up_the_description_from_the_configuration_type_in_the_assembly()
        {
            theGroup.Description.ShouldEqual(new FubuDiagnosticsConfiguration().Description);
        }

        [Test]
        public void build_a_group_for_an_assembly_with_no_diagnostics_configuration()
        {
            var group = new DiagnosticGroup(typeof (TestFixtureAttribute).Assembly, new ActionCall[0]);

            group.Url.ShouldEqual("nunit.framework");
            group.Title.ShouldEqual("nunit.framework");
            group.Description.ShouldBeNull();
        }

        [Test]
        public void adds_chains_for_each_call()
        {
            var group = new DiagnosticGroup(GetType().Assembly);
            var descriptions = group.Chains.SelectMany(x => x.Calls).Select(x => x.Description).ToArray();

            group.Chains.Each(x => x.Group.ShouldBeTheSameAs(group));

            descriptions.ShouldContain("FakeFubuDiagnostics.Index() : String");
            descriptions.ShouldContain("FakeFubuDiagnostics.get_link() : String");
            descriptions.ShouldContain("FakeFubuDiagnostics.get_simple() : String");
            descriptions.ShouldContain("FakeFubuDiagnostics.get_query_Name(FakeQuery query) : String");

            
        }

        [Test]
        public void get_links()
        {
            var group = new DiagnosticGroup(GetType().Assembly);
            group.Links().Select(x => x.GetRoutePattern())
                .ShouldHaveTheSameElementsAs("_fubu/fake/link", "_fubu/fake/else", "_fubu/fake/simple");
        }

        [Test]
        public void if_index_exists_use_that_as_the_default()
        {
            var indexCall = ActionCall.For<FakeFubuDiagnostics>(x => x.Index());
            var call1 = ActionCall.For<FakeFubuDiagnostics>(x => x.get_simple());
            var call2 = ActionCall.For<FakeFubuDiagnostics>(x => x.get_link());

            var group = new DiagnosticGroup(Assembly.GetExecutingAssembly(), new ActionCall[] {indexCall, call1, call2});

            group.GetDefaultUrl().ShouldEqual("_fubu/fake");

        }

        [Test]
        public void if_only_one_link_use_that_as_the_default()
        {
            var call1 = ActionCall.For<FakeFubuDiagnostics>(x => x.get_simple());

            var group = new DiagnosticGroup(Assembly.GetExecutingAssembly(), new ActionCall[] { call1 });

            group.GetDefaultUrl().ShouldEqual("_fubu/fake/simple");
        }

        [Test]
        public void if_no_index_use_the_derived_index()
        {
            var call1 = ActionCall.For<FakeFubuDiagnostics>(x => x.get_simple());
            var call2 = ActionCall.For<FakeFubuDiagnostics>(x => x.get_link());

            var group = new DiagnosticGroup(Assembly.GetExecutingAssembly(), new ActionCall[] { call1, call2 });

            group.GetDefaultUrl().ShouldEqual("_fubu/fake");

        }
    }

    public class FubuDiagnosticsConfiguration
    {
        public string Url = "fake";
        public string Title = "Fake Diagnostics";
        public string Description = "Some fake diagnostics";
    }
}