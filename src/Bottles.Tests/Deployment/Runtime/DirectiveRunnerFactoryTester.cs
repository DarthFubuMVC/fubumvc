using System;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using FubuCore;

namespace Bottles.Tests.Deployment.Runtime
{
    [TestFixture]
    public class DirectiveRunnerFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void GivenAContainerWith()
        {
            IContainer container = new Container(cfg =>
            {
                cfg.For<IInitializer<FakeDirective>>()
                    .Use<FakeInitializer<FakeDirective>>();
                cfg.For<IInitializer<FakeDirective>>()
                    .Use<FakeInitializer2<FakeDirective>>();


                cfg.For<IDeployer<FakeDirective>>()
                    .Use<FakeDeployer<FakeDirective>>();
                cfg.For<IDeployer<FakeDirective>>()
                    .Use<FakeDeployer2<FakeDirective>>();
                cfg.For<IDeployer<FakeDirective>>()
                    .Use<FakeDeployer2<FakeDirective>>();

                cfg.For<IFinalizer<FakeDirective>>()
                    .Use<FakeFinalizer<FakeDirective>>();

                cfg.For<IDeploymentDiagnostics>().Use<FakeDeploymentDiagnostics>();
            });

            var factory = new DirectiveRunnerFactory(container);
            theRunner = (DirectiveRunner<FakeDirective>)factory.Build(new FakeDirective());
        }

        #endregion

        private DirectiveRunner<FakeDirective> theRunner;

        [Test]
        public void should_have_2_initializers()
        {
            theRunner.Initializers.ShouldHaveCount(2);
        }

        [Test]
        public void should_have_3_deployers()
        {
            theRunner.Deployers.ShouldHaveCount(3);
        }

        [Test]
        public void should_have_1_finalizer()
        {
            theRunner.Finalizers.ShouldHaveCount(1);
        }
    }

    public class FakeInitializer<T> : IInitializer<T> where T : IDirective
    {
        public void Execute(T directive, HostManifest host, IPackageLog log)
        {
            directive.As<FakeDirective>().HitIt();
        }
    }

    public class FakeInitializer2<T> : FakeInitializer<T> where T : IDirective
    {
    }

    public class FakeDeployer<T> : IDeployer<T> where T : IDirective
    {
        public void Execute(T directive, HostManifest host, IPackageLog log)
        {
            directive.As<FakeDirective>().HitIt();
        }
    }

    public class FakeDeployer2<T> : FakeDeployer<T> where T : IDirective
    {
    }

    public class FakeDeployer3<T> : FakeDeployer<T> where T : IDirective
    {
    }

    public class FakeFinalizer<T> : IFinalizer<T> where T : IDirective
    {
        public void Execute(T directive, HostManifest host, IPackageLog log)
        {
            directive.As<FakeDirective>().HitIt();
        }
    }
}