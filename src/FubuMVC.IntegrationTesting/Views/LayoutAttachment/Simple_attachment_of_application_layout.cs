using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.LayoutAttachment
{
    [TestFixture]
    public class Simple_attachment_of_application_layout : ViewIntegrationContext
    {
        public Simple_attachment_of_application_layout()
        {
            RazorView("Shared/Application").Write("Some content");

            RazorView<ViewModel1>("View1");
            RazorView<ViewModel2>("Folder1/View2");
            RazorView<ViewModel3>("Folder1/Folder2/View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");
        }

        [Test]
        public void should_have_exactly_5_views()
        {
            Views.Templates<RazorTemplate>().Count().ShouldEqual(5);
        }

        [Test]
        public void all_views_have_the_main_application_master()
        {

            var master = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "Application");
            master.ShouldNotBeNull();

            Views.Templates<RazorTemplate>().Where(x => x != master).Each(view => {
                view.Master.ShouldBeTheSameAs(master);
            });
        }

        [Test]
        public void master_layout_itself_will_get_no_layout_in_this_case()
        {
            var master = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "Application");
            master.Master.ShouldBeNull();
        }
    }
}