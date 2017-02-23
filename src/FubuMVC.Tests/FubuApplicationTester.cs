using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class FubuApplicationTester
    {
        public FubuApplicationTester()
        {
            var system = new FileSystem();
            system.FindFiles(".".ToFullPath(), new FileSet
            {
                Include = "*.asset.config;*.script.config"
            }).ToList().Each(system.DeleteFile);
        }


        [Fact]
        public void the_restarted_property_is_set()
        {
            var floor = DateTime.Now.AddSeconds(-5);
            var ceiling = DateTime.Now.AddSeconds(5);


            using (var runtime = FubuRuntime.Basic())
            {
                (floor < runtime.Restarted && runtime.Restarted < ceiling).ShouldBeTrue();
            }
        }

        [Fact]
        public void description_smoke_tester()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                var description = FubuApplicationDescriber.WriteDescription(runtime.ActivationDiagnostics, runtime);

                Console.WriteLine(description);
            }
        }

        [Fact]
        public void can_use_the_default_policies()
        {
            var application = FubuRuntime.Basic();
            var graph = application.Get<BehaviorGraph>();

            graph.ChainFor<TargetEndpoint>(x => x.get_hello()).ShouldNotBeNull();
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