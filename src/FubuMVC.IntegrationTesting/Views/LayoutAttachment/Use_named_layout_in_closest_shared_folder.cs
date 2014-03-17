using System.Linq;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.LayoutAttachment
{
    [TestFixture]
    public class Use_named_layout_in_closest_shared_folder : ViewIntegrationContext
    {
        public Use_named_layout_in_closest_shared_folder()
        {
            RazorView("Shared/Application");
            RazorView("Folder1/Shared/Application");
            RazorView("Folder1/Folder2/Shared/Application");

            RazorView<ViewModel1>("View1");
            RazorView<ViewModel1>("Folder1/View2");
            RazorView<ViewModel1>("Folder1/Folder2/View3");
            RazorView<ViewModel1>("Folder1/Folder2/Folder3/Folder4/View4");
        }

        [Test]
        public void use_the_named_layout_in_the_same_directorys_shared_folder_if_it_exists()
        {
            Views.Templates<RazorTemplate>()
                .FirstOrDefault(x => x.Name() == "View1")
                .Master.ViewPath.ShouldEqual("Shared/Application.cshtml");

            Views.Templates<RazorTemplate>()
                .FirstOrDefault(x => x.Name() == "View2")
                .Master.ViewPath.ShouldEqual("Folder1/Shared/Application.cshtml");

            Views.Templates<RazorTemplate>()
                .FirstOrDefault(x => x.Name() == "View3")
                .Master.ViewPath.ShouldEqual("Folder1/Folder2/Shared/Application.cshtml");
        }

        [Test]
        public void search_recursively_up_the_folder_structure_to_find_named_layout()
        {
            Views.Templates<RazorTemplate>()
                .FirstOrDefault(x => x.Name() == "View4")
                .Master.ViewPath.ShouldEqual("Folder1/Folder2/Shared/Application.cshtml");
        }


    }
}