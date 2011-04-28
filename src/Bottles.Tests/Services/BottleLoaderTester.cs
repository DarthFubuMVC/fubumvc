using System;
using System.Linq;
using Bottles.Deployment;
using Bottles.Exploding;
using Bottles.Services;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Services
{
    [TestFixture]
    public class BottleLoaderTester : InteractionContext<BottleHostLoader>
    {
        protected override void beforeEach()
        {
        }

        void theActualBottlesAre(params string[] bottles)
        {
            MockFor<IPackageExploder>().Stub(
                x => x.ExplodeAllZipsAndReturnPackageDirectories(AppDomain.CurrentDomain.BaseDirectory)).Return(bottles);
            foreach (var bottle in bottles)
            {
                MockFor<IFileSystem>().Stub(x => x.GetFullPath(bottle)).Return(bottle);
                MockFor<IFileSystem>().Stub(x => x.LoadFromFile<ServiceHostManifest>(bottle, ServiceHostManifest.CONTROL, ServiceHostManifest.FILE))
                    .Return(new ServiceHostManifest() { Name = bottle });    
            }
        }

        [Test]
        public void should_be_two_packages()
        {
            theActualBottlesAre("a","b");
            var packages = ClassUnderTest.Load();
            packages.ShouldHaveCount(2);
        }

        [Test]
        public void first_should_be_a()
        {
            theActualBottlesAre("a","b");
            var package = ClassUnderTest.Load().First();
            package.Name.ShouldEqual("a");
        }

        [Test]
        public void there_should_be_a_data_folder()
        {
            theActualBottlesAre("a");

            var package = ClassUnderTest.Load().First();
            bool dataExists=false;
            package.ForFolder("data",n=>dataExists = true);
            dataExists.ShouldBeTrue();
        }

        [Test]
        public void there_should_be_a_control_folder()
        {
            theActualBottlesAre("a");

            var package = ClassUnderTest.Load().First();
            bool exists = false;
            package.ForFolder("control", n => exists = true);
            exists.ShouldBeTrue();
        }

        [Test]
        public void there_should_not_be_a_bob_folder()
        {
            theActualBottlesAre("a");

            var package = ClassUnderTest.Load().First();
            bool dataExists = false;
            package.ForFolder("bob", n => dataExists = true);
            dataExists.ShouldBeFalse();
        }
        
    }
}