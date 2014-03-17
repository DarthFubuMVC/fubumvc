using System.Linq;
using FubuCore;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.LayoutAttachment
{
    [TestFixture]
    public class partials_do_not_get_layouts : ViewIntegrationContext
    {
        public partials_do_not_get_layouts()
        {
            RazorView("Shared/Application").Write("Some content");

            RazorView<ViewModel1>("View1");
            RazorView<ViewModel2>("Folder1/_View2");
            RazorView<ViewModel3>("Folder1/Folder2/View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");
        }

        [Test]
        public void partial_does_not_get_a_layout()
        {
            var view = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "_View2");
            view.As<RazorTemplate>().IsPartial().ShouldBeTrue();

            view.Master.ShouldBeNull();
        }
    }
}