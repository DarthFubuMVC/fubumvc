using System;
using System.Diagnostics;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using StructureMap;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;
using FubuCore.Descriptions;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuApplicationTester
    {
        [SetUp]
        public void SetUp()
        {
            var system = new FileSystem();
            system.FindFiles(".".ToFullPath(), new FileSet
            {
                Include = "*.asset.config;*.script.config"
            }).ToList().Each(system.DeleteFile);
 
        }

        [Test]
        public void icontainer_facility_is_registered_during_construction_for_later()
        {

            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();

            container.GetInstance<IContainerFacility>()
                .ShouldBeOfType<StructureMapContainerFacility>()
                .ShouldNotBeNull();
        }

        [Test]
        public void the_restarted_property_is_set()
        {
            var floor = DateTime.Now.AddSeconds(-5);
            var ceiling = DateTime.Now.AddSeconds(5);

            FubuMvcPackageFacility.Restarted = null;

            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();

            (floor < FubuMvcPackageFacility.Restarted && FubuMvcPackageFacility.Restarted < ceiling).ShouldBeTrue();

        }

        [Test]
        public void description_smoke_tester()
        {
            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();

            var description = FubuApplicationDescriber.WriteDescription();

            Debug.WriteLine(description);

        }
    }
}   