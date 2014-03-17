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
    public class override_the_shared_folder_name : ViewIntegrationContext
    {
        public override_the_shared_folder_name()
        {
            RazorView("Layouts/Application").Write("Some content");
            RazorView("Shared/Application").Write("Some content");
            RazorView("Layouts/Theme").Write("Some content");

            RazorView<ViewModel1>("View1");
            RazorView<ViewModel2>("Folder1/View2");
            RazorView<ViewModel3>("Folder1/Folder2/View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");
        }

        public class MyRegistry : FubuRegistry
        {
            public MyRegistry()
            {
                AlterSettings<ViewEngineSettings>(x => x.SharedLayoutFolders.Add("Layouts"));
            }
        }


        [Test]
        public void all_views_have_the_main_application_master()
        {

            var master = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "Application" && x.RelativePath() == "Layouts/Application.cshtml");
            master.ShouldNotBeNull();

            Views.Templates<RazorTemplate>().Where(x => x != master && x.Name() != "Application").Each(view =>
            {
                view.Master.ShouldBeTheSameAs(master);
            });
        }
    }
}