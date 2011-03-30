using System;
using FubuFastPack.JqGrid;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class EntityDTOTester
    {
        [Test]
        public void id_method()
        {
            var dto = new EntityDTO();
            var guid = Guid.NewGuid();

            dto["Id"] = guid.ToString();

            dto.Id().ShouldEqual(guid);
        }
    }
}