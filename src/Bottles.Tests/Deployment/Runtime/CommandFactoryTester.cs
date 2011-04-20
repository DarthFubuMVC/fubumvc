using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using NUnit.Framework;
using StructureMap;
using FubuTestingSupport;

namespace Bottles.Tests.Deployment.Runtime
{
    [TestFixture]
    public class CommandFactoryTester
    {
        private ICommandFactory _commandFactory;

        [SetUp]
        public void GivenAContainerWith()
        {
            IContainer container = new Container(cfg=>
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

                                                     cfg.For<ILogger>().Use<FakeLogger>();
                                                 });

            _commandFactory = new CommandFactory(container);
        }

        [Test]
        public void should_have_2_initializers()
        {
            var fakeDirective = new FakeDirective();
            var output = _commandFactory.InitializersFor(fakeDirective);
            output.DeployWith(fakeDirective);

            fakeDirective.Hits.ShouldEqual(2);
        }

        [Test]
        public void should_have_3_deployers()
        {
            var fakeDirective = new FakeDirective();
            var output = _commandFactory.DeployersFor(fakeDirective);
            output.DeployWith(fakeDirective);

            fakeDirective.Hits.ShouldEqual(3);
        }

        [Test]
        public void should_have_1_finalizer()
        {
            var fakeDirective = new FakeDirective();
            var output = _commandFactory.FinalizersFor(fakeDirective);
            output.DeployWith(fakeDirective);

            fakeDirective.Hits.ShouldEqual(1);
        }


 
    }       
    
    public class FakeInitializer<T> : IInitializer<T> where T : IDirective
        {
            public void Initialize(IDirective directive)
            {
                var d = (FakeDirective)directive;
                d.HitIt();
            }
        }
        public class FakeInitializer2<T> : FakeInitializer<T> where T : IDirective { }

        public class FakeDeployer<T> : IDeployer<T> where T : IDirective
        {
            public void Deploy(IDirective directive)
            {
                var d = (FakeDirective)directive;
                d.HitIt();
            }
        }
        public class FakeDeployer2<T> : FakeDeployer<T> where T : IDirective { }
        public class FakeDeployer3<T> : FakeDeployer<T> where T : IDirective { }

        public class FakeFinalizer<T> : IFinalizer<T> where T : IDirective
        {
            public void Finish(IDirective directive)
            {
                var d = (FakeDirective)directive;
                d.HitIt();
            }
        }
}