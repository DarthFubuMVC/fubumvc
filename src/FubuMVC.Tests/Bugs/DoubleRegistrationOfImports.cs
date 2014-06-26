using System;
using System.Diagnostics;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.Tests.Bugs
{
    [TestFixture]
    public class DoubleRegistrationOfImports
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void an_extension_is_only_applied_once()
        {
            OneExtension.Applied = TwoExtension.Applied = ThreeExtension.Applied = 0;

            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Import<SubModule>();
                x.Import<OneExtension>();
                x.Import<TwoExtension>();
                x.Import<ThreeExtension>();
            });

            OneExtension.Applied.ShouldEqual(1);
            TwoExtension.Applied.ShouldEqual(1);
            ThreeExtension.Applied.ShouldEqual(1);
        }

        [Test]
        public void a_fubu_package_registry_is_only_imported_once()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Import<SubModule>();
                x.Import<OneExtension>();
                x.Import<TwoExtension>();
                x.Import<ThreeExtension>();
            });

            var actions = graph.Actions().Where(x => x.HandlerType == typeof (SomeClassWithActions));


            actions.ShouldHaveCount(1);
        }

        [Test]
        public void do_not_allow_second_import_to_be_processed()
        {
            // SubModule2 tries to import SubModule1 with the same url prefix
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Import<SubModule>();
                x.Import<SubModule2>();
            });

            var actions = graph.Actions().Where(x => x.HandlerType == typeof(SomeClassWithActions));


            actions.ShouldHaveCount(1);
        }

        [Test]
        public void policies_will_only_be_applied_once()
        {
            SpecialPolicy.Applied = 0;

            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Policies.Local.Add<SpecialPolicy>();
                x.Import<OneExtension>();
                x.Import<TwoExtension>();
                x.Import<ThreeExtension>();
            });

            SpecialPolicy.Applied.ShouldEqual(1);
        }
    }

    public class SpecialPolicy : IConfigurationAction
    {
        public static int Applied;

        public void Configure(BehaviorGraph graph)
        {
            Applied++;
        }
    }

    public class SubModule2 : FubuPackageRegistry
    {
        public SubModule2()
        {
            Import<SubModule>();
        }
    }

    public class SubModule : FubuPackageRegistry
    {
        public SubModule()
        {
            Actions.IncludeType<SomeClassWithActions>();
            //Route(Guid.NewGuid().ToString()).Calls<SomeClassWithActions>(x => x.get_hello());
        }
    }

    public class SomeClassWithActions
    {
        public string get_hello()
        {
            return "Hello.";
        }
    }

    public class OneExtension : IFubuRegistryExtension
    {
        public static int Applied;

        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Local.Add<SpecialPolicy>();
            registry.Import<SubModule>();

            Applied++;
        }
    }

    public class TwoExtension : IFubuRegistryExtension
    {
        public static int Applied;

        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Local.Add<SpecialPolicy>();
            registry.Import<SubModule>();
            Applied++;
        }
    }

    public class ThreeExtension : IFubuRegistryExtension
    {
        public static int Applied;

        public void Configure(FubuRegistry registry)
        {
            registry.Import<SubModule>();
            registry.Import<TwoExtension>();
            registry.Import<OneExtension>();

            Applied++;
        }
    }
}