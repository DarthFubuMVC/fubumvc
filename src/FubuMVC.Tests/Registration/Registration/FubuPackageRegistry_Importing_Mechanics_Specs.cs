using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.Registration.Conventions;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Registration
{
    [TestFixture]
    public class FubuPackageRegistry_Importing_Mechanics_Specs
    {
        private FubuRuntime _runtime;
        private BehaviorGraph behaviors;
        private BehaviorChain appChain;
        private BehaviorChain pakChain;

        [SetUp]
        public void SetUp()
        {
            _runtime = FubuRuntime.For<ApplicationRegistry>();
            behaviors = _runtime.Get<BehaviorGraph>();

            appChain = behaviors.ChainFor<ApplicationActions>(x => x.get_app_action());
            pakChain = behaviors.ChainFor<PackageActions>(x => x.get_pak_action());
        }

        [TearDown]
        public void TearDown()
        {
            _runtime.Dispose();
        }

        [Test]
        public void the_application_service_registrations_win()
        {
            _runtime.Get<IAppService>().ShouldBeOfType<AppService>();
        }

        [Test]
        public void local_policy_in_the_main_app_is_only_applied_to_endpoints_in_the_main_app()
        {
            appChain.IsWrappedBy(typeof(LocalAppWrapper)).ShouldBeTrue();
            pakChain.IsWrappedBy(typeof(LocalAppWrapper)).ShouldBeFalse();
        }

        [Test]
        public void local_policy_in_the_import_is_only_applied_to_endpoints_in_the_import()
        {
            appChain.IsWrappedBy(typeof(LocalPakWrapper)).ShouldBeFalse();
            pakChain.IsWrappedBy(typeof(LocalPakWrapper)).ShouldBeTrue(); 
        }

        [Test]
        public void global_policy_applied_by_the_app_is_applied_to_all_chains()
        {
            appChain.IsWrappedBy(typeof(GlobalAppWrapper)).ShouldBeTrue();
            pakChain.IsWrappedBy(typeof(GlobalAppWrapper)).ShouldBeTrue(); 
        }

        [Test]
        public void global_policy_applied_by_an_import_is_applied_to_all_chains()
        {
            appChain.IsWrappedBy(typeof(GlobalPakWrapper)).ShouldBeTrue();
            pakChain.IsWrappedBy(typeof(GlobalPakWrapper)).ShouldBeTrue(); 
        }
    }

    public class LocalAppWrapper : FubuMVC.Core.Behaviors.WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            
        }
    }

    public class GlobalAppWrapper : FubuMVC.Core.Behaviors.WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            throw new NotImplementedException();
        }
    }

    public class GlobalPakWrapper : FubuMVC.Core.Behaviors.WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            throw new NotImplementedException();
        }
    }

    public class LocalPakWrapper : FubuMVC.Core.Behaviors.WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            throw new NotImplementedException();
        }
    }

    public class ApplicationRegistry : FubuRegistry
    {
        public ApplicationRegistry()
        {
            Actions.IncludeType<ApplicationActions>();

            Import<PackageRegistry>();


            Policies.Local.Configure(graph =>
            {
                graph.WrapAllWith<LocalAppWrapper>();
            });

            Policies.Global.Configure(graph =>
            {
                graph.WrapAllWith<GlobalAppWrapper>();
            });

            Services.ReplaceService<IAppService, AppService>();
        }
    }

    public class PackageRegistry : FubuPackageRegistry
    {
        public PackageRegistry()
        {
            Actions.IncludeType<PackageActions>();

            Policies.Local.Configure(graph =>
            {
                graph.WrapAllWith<LocalPakWrapper>();
            });

            Policies.Global.Configure(x =>
            {
                x.WrapAllWith<GlobalPakWrapper>();
            });

            Services.ReplaceService<IAppService, PackageService>();

            Services.AddService<IDeactivator, PackageDeactivator>();
        }
    }

    public class PackageDeactivator : IDeactivator
    {
        public void Deactivate(IActivationLog log)
        {
            throw new NotImplementedException();
        }
    }

    public class PackageActions
    {
        public string get_pak_action()
        {
            return "anything";
        }
    }

    public class ApplicationActions
    {
        public string get_app_action()
        {
            return "anything";
        }
    }

    public interface IAppService{}
    public class AppService : IAppService{}
    public class PackageService : IAppService{}
}