using System;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Runtime
{
    [TestFixture]
    public class when_attaching_to_a_runner : InteractionContext<DirectiveRunner<OneDirective>>
    {
        private HostManifest theHost;
        private OneDirective theDirective;

        protected override void beforeEach()
        {
            theHost = new HostManifest("something");
            theDirective = new OneDirective();
        
            ClassUnderTest.Attach(theHost, theDirective);
        }

        [Test]
        public void logs_the_directive_against_the_host()
        {
            MockFor<IDeploymentDiagnostics>().AssertWasCalled(x => x.LogDirective(theHost, theDirective));
        }
    }

    [TestFixture]
    public class when_executing_the_initializers : InteractionContext<DirectiveRunner<OneDirective>>
    {
        private HostManifest theHost;
        private OneDirective theDirective;
        private IInitializer<OneDirective>[] theInitializers;
        private PackageLog[] theLogs;

        protected override void beforeEach()
        {
            theHost = new HostManifest("something");
            theDirective = new OneDirective();

            

            theInitializers = Services.CreateMockArrayFor<IInitializer<OneDirective>>(5);

            theLogs = new PackageLog[5];
            for (int i = 0; i < theLogs.Length; i++)
            {
                theLogs[i] = new PackageLog();
                MockFor<IDeploymentDiagnostics>().Expect(x => x.LogAction(theHost, theDirective, theInitializers[i]))
                    .Return(theLogs[i]);

                
            }

            ClassUnderTest.Attach(theHost, theDirective);
            ClassUnderTest.InitializeDeployment();
        }

        [Test]
        public void all_the_initializers_were_logged()
        {
            MockFor<IDeploymentDiagnostics>().VerifyAllExpectations();
        }

        [Test]
        public void all_the_inializers_were_executed_with_the_correct_log()
        {
            for (int i = 0; i < theInitializers.Length; i++)
            {
                theInitializers[i].AssertWasCalled(x => x.Execute(theDirective, theHost, theLogs[i]));
            }
        }
    }


    [TestFixture]
    public class when_executing_the_deployers : InteractionContext<DirectiveRunner<OneDirective>>
    {
        private HostManifest theHost;
        private OneDirective theDirective;
        private IDeployer<OneDirective>[] theDeployers;
        private PackageLog[] theLogs;

        protected override void beforeEach()
        {
            theHost = new HostManifest("something");
            theDirective = new OneDirective();



            theDeployers = Services.CreateMockArrayFor<IDeployer<OneDirective>>(5);

            theLogs = new PackageLog[5];
            for (int i = 0; i < theLogs.Length; i++)
            {
                theLogs[i] = new PackageLog();
                MockFor<IDeploymentDiagnostics>().Expect(x => x.LogAction(theHost, theDirective, theDeployers[i]))
                    .Return(theLogs[i]);


            }

            ClassUnderTest.Attach(theHost, theDirective);
            ClassUnderTest.Deploy();
        }

        [Test]
        public void all_the_Deployers_were_logged()
        {
            MockFor<IDeploymentDiagnostics>().VerifyAllExpectations();
        }

        [Test]
        public void all_the_Deployers_were_executed_with_the_correct_log()
        {
            for (int i = 0; i < theDeployers.Length; i++)
            {
                theDeployers[i].AssertWasCalled(x => x.Execute(theDirective, theHost, theLogs[i]));
            }
        }
    }

    [TestFixture]
    public class when_executing_the_Finalizers : InteractionContext<DirectiveRunner<OneDirective>>
    {
        private HostManifest theHost;
        private OneDirective theDirective;
        private IFinalizer<OneDirective>[] theFinalizers;
        private PackageLog[] theLogs;

        protected override void beforeEach()
        {
            theHost = new HostManifest("something");
            theDirective = new OneDirective();



            theFinalizers = Services.CreateMockArrayFor<IFinalizer<OneDirective>>(5);

            theLogs = new PackageLog[5];
            for (int i = 0; i < theLogs.Length; i++)
            {
                theLogs[i] = new PackageLog();
                MockFor<IDeploymentDiagnostics>().Expect(x => x.LogAction(theHost, theDirective, theFinalizers[i]))
                    .Return(theLogs[i]);


            }

            ClassUnderTest.Attach(theHost, theDirective);
            ClassUnderTest.FinalizeDeployment();
        }

        [Test]
        public void all_the_Finalizers_were_logged()
        {
            MockFor<IDeploymentDiagnostics>().VerifyAllExpectations();
        }

        [Test]
        public void all_the_Finalizers_were_executed_with_the_correct_log()
        {
            for (int i = 0; i < theFinalizers.Length; i++)
            {
                theFinalizers[i].AssertWasCalled(x => x.Execute(theDirective, theHost, theLogs[i]));
            }
        }
    }

}