using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bottles.Deployment;
using Bottles.Deployment.Runtime.Content;
using Bottles.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Runtime.Content
{
    [TestFixture]
    public class BottleMoverTester : InteractionContext<BottleMover>
    {
        private List<BottleReference> theReferences;
        private List<PackageManifest> theManifests;
        private List<BottleExplosionRequest> theExplosionRequests;

        protected override void beforeEach()
        {
            theReferences = new List<BottleReference>();
            theManifests = new List<PackageManifest>();
            theExplosionRequests = new List<BottleExplosionRequest>();
        
        
            addBottle("A", 2);
            addBottle("B", 1);
            addBottle("C", 4);

            ClassUnderTest.Move(MockFor<IBottleDestination>(), theReferences);
        }

        private void addBottle(string name, int numberOfExplosionRequests)
        {
            theReferences.Add(new BottleReference(name));

            var manifest = new PackageManifest();
            MockFor<IBottleRepository>().Stub(x => x.ReadManifest(name)).Return(manifest);
            theManifests.Add(manifest);

            var list = new List<BottleExplosionRequest>();
            for (int i = 0; i < numberOfExplosionRequests; i++)
            {
                list.Add(new BottleExplosionRequest(new PackageLog()){
                    BottleName = name,
                    DestinationDirectory = "name:" + i
                });
            }

            MockFor<IBottleDestination>().Stub(x => x.DetermineExplosionRequests(manifest))
                .Return(list);


            theExplosionRequests.AddRange(list);
        }

        [Test]
        public void should_have_processed_all_the_explosion_requests_for_all_the_manifests_from_the_bottle_references()
        {
            theExplosionRequests.Each(r =>
            {
                MockFor<IBottleRepository>().AssertWasCalled(x => x.ExplodeFiles(r));
            });
        }
    }
}