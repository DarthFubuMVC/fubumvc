using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.View;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.LayoutAttachment
{
    [TestFixture]
    public class override_the_default_layout_name : ViewIntegrationContext
    {
        public override_the_default_layout_name()
        {
            RazorView("Shared/Application").Write("Some content");
            RazorView("Shared/Theme").Write("Some content");

            RazorView<ViewModel1>("View1");
            RazorView<ViewModel2>("Folder1/View2");
            RazorView<ViewModel3>("Folder1/Folder2/View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");


        }

        public class MyRegistry : FubuRegistry
        {
            public MyRegistry()
            {
                AlterSettings<ViewEngineSettings>(x => x.ApplicationLayoutName = "Theme");
            }
        }

        [Test]
        public void all_views_have_the_main_application_master_called_Theme_from_the_override()
        {

            var master = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "Theme");
            master.ShouldNotBeNull();

            Views.Templates<RazorTemplate>().Where(x => x != master).Each(view =>
            {
                view.Master.ShouldBeTheSameAs(master);
            });
        }
    }
}