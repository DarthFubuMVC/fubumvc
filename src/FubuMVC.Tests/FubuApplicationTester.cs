using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Bootstrapping;
using NUnit.Framework;
using StructureMap;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuApplicationTester
    {
        [Test]
        public void icontainer_facility_is_registered_during_construction_for_later()
        {
            var system = new FileSystem();
            system.FindFiles(".".ToFullPath(), new FileSet{
                Include = "*.asset.config;*.script.config"
            }).ToList().Each(system.DeleteFile);

            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();

            container.GetInstance<IContainerFacility>()
                .ShouldBeOfType<StructureMapContainerFacility>()
                .ShouldNotBeNull();
        }


    }
}   