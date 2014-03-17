using System.Linq;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.LayoutAttachment
{
    [TestFixture]
    public class Explicit_declaration_of_master_layout : ViewIntegrationContext
    {
        public Explicit_declaration_of_master_layout()
        {
            RazorView("Shared/Application").Write("Some content");
            RazorView("Shared/Special").Write("some content");

            RazorView<ViewModel1>("View1").Write("@layout Special");
            RazorView<ViewModel2>("Folder1/View2");
            RazorView<ViewModel3>("Folder1/Folder2/View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");
        }

        [Test]
        public void use_the_explicit_master_name_if_it_exists()
        {
            var view1 = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "View1");

            view1.Master.ShouldNotBeNull();
            view1.Master.Name().ShouldEqual("Special");
        }
    }
}