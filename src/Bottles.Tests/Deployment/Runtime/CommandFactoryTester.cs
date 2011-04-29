using System;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using NUnit.Framework;
using StructureMap;
using FubuCore;

namespace Bottles.Tests.Deployment.Runtime
{
    [TestFixture]
    public class CommandFactoryTester
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

            _commandFactory = new CommandFactory(container);
        }

        #endregion

        private ICommandFactory _commandFactory;

        [Test]
        public void TESTNAME()
        {
            Assert.Fail("NWO");
        }

        //[Test]
        //public void should_have_2_initializers()
        //{
        //    var fakeDirective = new FakeDirective();
        //    var output = _commandFactory.InitializersFor(fakeDirective);
        //    output.Process(null, fakeDirective);

        //    fakeDirective.Hits.ShouldEqual(2);
        //}

        //[Test]
        //public void should_have_3_deployers()
        //{
        //    var fakeDirective = new FakeDirective();
        //    var output = _commandFactory.DeployersFor(fakeDirective);
        //    output.Process(null, fakeDirective);

        //    fakeDirective.Hits.ShouldEqual(3);
        //}

        //[Test]
        //public void should_have_1_finalizer()
        //{
        //    var fakeDirective = new FakeDirective();
        //    var output = _commandFactory.FinalizersFor(fakeDirective);
        //    output.Process(null, fakeDirective);

        //    fakeDirective.Hits.ShouldEqual(1);
        //}
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