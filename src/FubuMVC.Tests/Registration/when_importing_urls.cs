using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    public class RegistryImportEndpoint
    {
        public string get_method1_Name_Age(RegistryImportInput input)
        {
            return "Hi.";
        }

        public string get_method2_Name_Age(RegistryImportInput input)
        {
            return "Hi.";
        }

        public string get_method3_Name_Age(RegistryImportInput input)
        {
            return "Hi.";
        }

        [FubuPartial]
        public void GoPartial(RegistryImportInput input)
        {
            
        }
    }

    public class RegistryImportInput
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [TestFixture]
    public class when_importing_urls
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            registry1 = new FubuRegistry();
            registry1.Actions.IncludeType<RegistryImportEndpoint>();

            theImport = new RegistryImport
            {
                Prefix = "area1",
                Registry = registry1
            };

            graph2 = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<TestController>();
            });

            theImport.InitializeSettings(graph2);
            graph2.As<IChainImporter>().Import(theImport.BuildChains(graph2));
        }

        #endregion

        private BehaviorGraph graph1;
        private BehaviorGraph graph2;
        private BehaviorChain chain;
        private FubuRegistry registry1;
        private RegistryImport theImport;

        [Test]
        public void should_have_all_the_routes_from_the_imported_graph()
        {
            graph2.Routes.Any(x => x.Pattern == "area1/method1/{Name}/{Age}").ShouldBeTrue();
            graph2.Routes.Any(x => x.Pattern == "area1/method2/{Name}/{Age}").ShouldBeTrue();
            graph2.Routes.Any(x => x.Pattern == "area1/method3/{Name}/{Age}").ShouldBeTrue();
        }

        [Test]
        public void should_have_imported_the_behavior_chains_without_routes()
        {
            graph2.BehaviorFor<RegistryImportEndpoint>(x => x.GoPartial(null))
                .ShouldNotBeNull();
        }
    }
}