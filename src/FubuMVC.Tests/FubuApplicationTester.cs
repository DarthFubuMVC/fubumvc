using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.StructureMap;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuApplicationTester
    {
        [SetUp]
        public void SetUp()
        {
            var system = new FileSystem();
            system.FindFiles(".".ToFullPath(), new FileSet
            {
                Include = "*.asset.config;*.script.config"
            }).ToList().Each(system.DeleteFile);
        }

        [Test]
        public void the_restarted_property_is_set()
        {
            var floor = DateTime.Now.AddSeconds(-5);
            var ceiling = DateTime.Now.AddSeconds(5);

            FubuApplication.Restarted = null;

            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();

            (floor < FubuApplication.Restarted && FubuApplication.Restarted < ceiling).ShouldBeTrue();
        }

        [Test]
        public void description_smoke_tester()
        {
            var container = new Container();
            using (var runtime = FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap())
            {
                var description = FubuApplicationDescriber.WriteDescription(runtime.Behaviors.Diagnostics);

                Console.WriteLine(description);
            }


        }

        [Test]
        public void can_use_the_default_policies()
        {
            var application = FubuApplication.DefaultPolicies().StructureMap(new Container()).Bootstrap();
            var graph = application.Factory.Get<BehaviorGraph>();

            graph.BehaviorFor<TargetEndpoint>(x => x.get_hello()).ShouldNotBeNull();
        }
    }

    public class TargetEndpoint
    {
        public string get_hello()
        {
            return "Hello";
        }
    }
}