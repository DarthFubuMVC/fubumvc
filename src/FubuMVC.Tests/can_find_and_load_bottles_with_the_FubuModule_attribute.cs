using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AssemblyPackage;
using Bottles;
using Bottles.PackageLoaders.Assemblies;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class can_find_and_load_bottles_with_the_FubuModule_attribute
    {
        [Test]
        public void find_assembly_bottles()
        {
            // Trash gets left over from other tests.  Joy.
            new FileSystem().DeleteFile("something.asset.config");
            new FileSystem().DeleteFile("something.script.config");
            new FileSystem().DeleteFile("else.script.config");
            new FileSystem().DeleteFile("else.asset.config");

            FubuApplication.For(new FubuRegistry()).StructureMap(new Container())
                .Bootstrap();

            var assembly = typeof (AssemblyPackageMarker).Assembly;

            PackageRegistry.PackageAssemblies.ShouldContain(assembly);


            PackageRegistry.Packages.OfType<AssemblyPackageInfo>().Any(
                x => x.Name == new AssemblyPackageInfo(assembly).Name)
                .ShouldBeTrue();
        }
    }
}