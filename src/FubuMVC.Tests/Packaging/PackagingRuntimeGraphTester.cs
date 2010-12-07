using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Packaging
{

    [TestFixture]
    public class when_adding_an_bootstrapper : InteractionContext<PackagingRuntimeGraph>
    {
        private StubBootstrapper bootstrapper1;
        private StubBootstrapper bootstrapper2;
        private StubBootstrapper bootstrapper3;

        protected override void beforeEach()
        {
            bootstrapper1 = new StubBootstrapper("a");
            bootstrapper2 = new StubBootstrapper("b");
            bootstrapper3 = new StubBootstrapper("c");

            ClassUnderTest.PushProvenance("A");
            ClassUnderTest.AddBootstrapper(bootstrapper1);

            ClassUnderTest.PushProvenance("B");
            ClassUnderTest.AddBootstrapper(bootstrapper2);

            ClassUnderTest.PopProvenance();
            ClassUnderTest.AddBootstrapper(bootstrapper3);
        }


        [Test]
        public void should_register_the_first_bootstrapper()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(bootstrapper1, "A"));
        }

        [Test]
        public void should_register_the_second_bootstrapper_with_nested_provenance()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(bootstrapper2, "A/B"));
        }

        [Test]
        public void should_register_the_third_bootstrapper_after_popping_the_provenance()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(bootstrapper3, "A"));
        }
    }


    [TestFixture]
    public class when_adding_an_activator : InteractionContext<PackagingRuntimeGraph>
    {
        private StubPackageActivator activator1;
        private StubPackageActivator activator2;
        private StubPackageActivator activator3;

        protected override void beforeEach()
        {
            activator1 = new StubPackageActivator();
            activator2 = new StubPackageActivator();
            activator3 = new StubPackageActivator();

            ClassUnderTest.PushProvenance("A");
            ClassUnderTest.AddActivator(activator1);

            ClassUnderTest.PushProvenance("B");
            ClassUnderTest.AddActivator(activator2);

            ClassUnderTest.PopProvenance();
            ClassUnderTest.AddActivator(activator3);
        }


        [Test]
        public void should_register_the_first_activator()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(activator1, "A"));
        }

        [Test]
        public void should_register_the_second_activator_with_nested_provenance()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(activator2, "A/B"));
        }

        [Test]
        public void should_register_the_third_activator_after_popping_the_provenance()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(activator3, "A"));
        }
    }


    [TestFixture]
    public class when_adding_a_package_loader : InteractionContext<PackagingRuntimeGraph>
    {
        private StubPackageLoader loader1;
        private StubPackageLoader loader2;
        private StubPackageLoader loader3;

        protected override void beforeEach()
        {
            loader1 = new StubPackageLoader("1a", "1b", "1c");
            loader2 = new StubPackageLoader("2a", "2b");
            loader3 = new StubPackageLoader("3a", "3b", "3c");
        
            ClassUnderTest.PushProvenance("A");
            ClassUnderTest.AddLoader(loader1);

            ClassUnderTest.PushProvenance("B");
            ClassUnderTest.AddLoader(loader2);

            ClassUnderTest.PopProvenance();
            ClassUnderTest.AddLoader(loader3);
        }

        [Test]
        public void should_register_the_first_loader()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(loader1, "A"));
        }

        [Test]
        public void should_register_the_second_loader_with_nested_provenance()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(loader2, "A/B"));
        }

        [Test] 
        public void should_register_the_third_loader_after_popping_the_provenance()
        {
            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogObject(loader3, "A"));
        }
    }

    [TestFixture]
    public class when_loading_and_logging_packages : InteractionContext<PackagingRuntimeGraph>
    {
        private StubPackageLoader loader1;
        private StubPackageLoader loader2;
        private StubPackageLoader loader3;

        [SetUp]
        public void SetUp()
        { 
            loader1 = new StubPackageLoader("1a", "1b", "1c");
            loader2 = new StubPackageLoader("2a", "2b");
            loader3 = new StubPackageLoader("3a", "3b", "3c");

            ClassUnderTest.AddLoader(loader1);
            ClassUnderTest.AddLoader(loader2);
            ClassUnderTest.AddLoader(loader3);

            ClassUnderTest.DiscoverAndLoadPackages(() => { });
        }


    }

    //[TestFixture]
    //public class end_to_end_PackageRuntimeGraph_compile_with_loaders : InteractionContext<PackagingRuntimeGraph>
    //{
    //    private StubPackageLoader loader1;
    //    private StubPackageLoader loader2;
    //    private StubPackageLoader loader3;

    //    protected override void beforeEach()
    //    {
    //        loader1 = new StubPackageLoader("1a", "1b", "1c");
    //        loader2 = new StubPackageLoader("2a", "2b", "2c");
    //        loader3 = new StubPackageLoader("3a", "3b", "3c");

    //        ClassUnderTest.AddLoader(loader1);
    //        ClassUnderTest.AddLoader(loader2);
    //        ClassUnderTest.AddLoader(loader3);


    //        ClassUnderTest.DiscoverAndLoadPackages();


    //    }


    //}


    public class StubPackageLoader : IPackageLoader
    {
        private readonly IEnumerable<IPackageInfo> _packages;

        public StubPackageLoader(params string[] names)
        {
            _packages = names.Select(x => new StubPackage(x) as IPackageInfo);
        }

        public IEnumerable<IPackageInfo> Load()
        {
            Thread.Sleep(101);
            return _packages;
        }

        public IEnumerable<IPackageInfo> Packages
        {
            get { return _packages; }
        }
    }

    public class StubPackage : IPackageInfo
    {
        private readonly string _name;

        public StubPackage(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public void LoadAssemblies(IAssemblyRegistration loader)
        {
            LoadingAssemblies(loader);
        }

        public void ForFolder(string folderName, Action<string> onFound)
        {
            // do nothing for now
        }

        public Action<IAssemblyRegistration> LoadingAssemblies { get; set; }
    }

    public class StubBootstrapper : IBootstrapper
    {
        private readonly string _name;
        private readonly IPackageActivator[] _activators;

        public StubBootstrapper(string name, params IPackageActivator[] activators)
        {
            _name = name;
            _activators = activators;
        }

        public IEnumerable<IPackageActivator> Bootstrap(IPackageLog log)
        {
            return _activators;
        }

        public override string ToString()
        {
            return string.Format("Bootstrapper: {0}", _name);
        }
    }

    public class StubPackageActivator : IPackageActivator
    {
        private IEnumerable<IPackageInfo> _packages;
        private IPackageLog _log;

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _packages = packages;
            _log = log;

            
        }

        public IEnumerable<IPackageInfo> Packages
        {
            get { return _packages; }
        }

        public IPackageLog Log
        {
            get { return _log; }
        }
    }
}