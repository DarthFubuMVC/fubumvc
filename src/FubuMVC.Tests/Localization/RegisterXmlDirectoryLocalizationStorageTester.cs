using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Environment;
using FubuLocalization.Basic;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class RegisterXmlDirectoryLocalizationStorageTester : InteractionContext<RegisterXmlDirectoryLocalizationStorage>
    {
        private IPackageInfo[] thePackages;
        private StubContainerFacility theFacility;

        protected override void beforeEach()
        {
            thePackages = new IPackageInfo[]{
                new ContentOnlyPackageInfo("dir1", "Dir1"),
                new ContentOnlyPackageInfo("dir2", "Dir2"),
                new ContentOnlyPackageInfo("dir3", "Dir3"),
            };

            theFacility = new StubContainerFacility();
            Services.Inject<IContainerFacility>(theFacility);

            var spinup = Services.Container.GetInstance<SpinUpLocalizationCaches>();
            theFacility.Container.Inject(spinup);

            ClassUnderTest.Activate(thePackages, new PackageLog());
        }

        [Test]
        public void should_register_an_xml_directory_storage_that_points_to_the_application_directory_and_the_package_directories()
        {
            var theStorage = theFacility.LocalizationStorageObjectDefThatWasRegistered.Value.ShouldBeOfType
                <XmlDirectoryLocalizationStorage>();
        
            theStorage.Directories.ShouldHaveTheSameElementsAs(FubuMvcPackageFacility.GetApplicationPath(), "dir1", "dir2", "dir3");
        }

        [Test]
        public void should_load_the_caches()
        {
            MockFor<ILocalizationProviderFactory>().AssertWasCalled(x => x.LoadAll(null), x => x.Constraints(Rhino.Mocks.Constraints.Is.NotNull()));
        }

        [Test]
        public void should_apply_the_factory_to_localization_manager()
        {
            MockFor<ILocalizationProviderFactory>().AssertWasCalled(x => x.ApplyToLocalizationManager());
        }
    }

    public class StubContainerFacility : IContainerFacility
    {
        public IBehaviorFactory BuildFactory()
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, ObjectDef def)
        {
            serviceType.ShouldEqual(typeof (ILocalizationStorage));
            LocalizationStorageObjectDefThatWasRegistered = def;
        }

        public ObjectDef LocalizationStorageObjectDefThatWasRegistered { get; set; }

        public void Inject(Type abstraction, Type concretion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IActivator> GetAllActivators()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInstaller> GetAllInstallers()
        {
            throw new NotImplementedException();
        }

        public T Get<T>()
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            throw new NotImplementedException();
        }

        private readonly Container _container = new Container();

        public Container Container
        {
            get { return _container; }
        }


    }
}