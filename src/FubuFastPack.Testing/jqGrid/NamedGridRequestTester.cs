using FubuFastPack.JqGrid;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class NamedGridRequestTester
    {
        [Test]
        public void for_static_builder()
        {
            NamedGridRequest.For<FakeCaseGrid>().GridName.ShouldEqual("FakeCase");
        }
    }
}