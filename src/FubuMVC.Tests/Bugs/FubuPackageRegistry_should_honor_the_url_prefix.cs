﻿using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Xunit;
using FubuCore;
using Shouldly;

namespace FubuMVC.Tests.Bugs
{
    
    public class FubuPackageRegistry_should_honor_the_url_prefix
    {
        [Fact]
        public void the_routes_from_FubuPackageRegistry_have_the_prefix()
        {
            var registry = new FubuRegistry();
            new SomePackageRegistry().As<IFubuRegistryExtension>().Configure(registry);

            var graph = BehaviorGraph.BuildFrom(registry);

            graph.ChainFor<SomeRandomClass>(x => x.get_some_data())
                .As<RoutedChain>()
                .GetRoutePattern().ShouldStartWith("mypak");
        }
    }

    public class SomePackageRegistry : FubuPackageRegistry
    {
        public SomePackageRegistry() : base("mypak")
        {
            Actions.IncludeType<SomeRandomClass>();
        }
    }

    public class SomeRandomClass
    {
        public string get_some_data()
        {
            return "Hello";
        }
    }
}