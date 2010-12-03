using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class when_loading_and_logging_packages
    {
        private StubPackageLoader loader1;
        private StubPackageLoader loader2;
        private StubPackageLoader loader3;
        private PackagingRuntimeGraph theGraph;

        [SetUp]
        public void SetUp()
        { 
            loader1 = new StubPackageLoader("1a", "1b", "1c");
            loader2 = new StubPackageLoader("2a", "2b");
            loader3 = new StubPackageLoader("3a", "3b", "3c");

            theGraph = new PackagingRuntimeGraph();

            theGraph.AddLoader(loader1);
            theGraph.AddLoader(loader2);
            theGraph.AddLoader(loader3);

            theGraph.Compile();
        }

        [Test]
        public void TESTNAME()
        {
            Assert.Fail("COME BACK HERE");
        }
    }

    public class StubPackageLoader : IPackageLoader
    {
        private readonly IEnumerable<IPackageInfo> _packages;

        public StubPackageLoader(params string[] names)
        {
            _packages = names.Select(x => new StubPackage(x));
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

        public void LoadAssemblies(IAssemblyLoader loader)
        {
            LoadingAssemblies(loader);
        }

        public Action<IAssemblyLoader> LoadingAssemblies { get; set; }
    }
}