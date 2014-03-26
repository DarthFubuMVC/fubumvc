using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class can_override_the_field_chrome_bug
    {
        public class TestRegistry : FubuRegistry
        {
            public TestRegistry()
            {
                Actions.IncludeType<ShowEditEndpoints>();
                Import<HtmlConventionRegistry>(x =>
                {
                    x.FieldChrome<TableRowFieldChrome>();
                });
            }
        }

        [Test]
        public void field_chrome_is_not_the_default()
        {
            using (var server = FubuApplication.For<TestRegistry>().StructureMap(new Container()).RunEmbedded())
            {
                server.Endpoints.GetByInput(new ShowModel { Name = "Jeremy" })
                         .ToString().ShouldEqual("<tr><td><label for=\"Name\">Name</label></td><td><span id=\"Name\">Jeremy</span></td></tr>");
            }
        }
    }
}