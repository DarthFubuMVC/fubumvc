using FubuCore;
using FubuMVC.Core.View.Model;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views
{
    [TestFixture]
    public class TemplateFileIntegrationTester : ViewIntegrationContext
    {
        private ITemplateFile view1;
        private ITemplateFile view2;
        private ITemplateFile view3;
        private ITemplateFile view4;

        public TemplateFileIntegrationTester()
        {
            RazorView<ViewModel1>("View1");
            RazorView<ViewModel2>("Folder1/View2");

            RazorView<ViewModel3>("View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");
        }

        protected override void beforeEach()
        {
            view1 = ViewForModel<ViewModel1>();
            view2 = ViewForModel<ViewModel2>();
            view3 = ViewForModel<ViewModel3>();
            view4 = ViewForModel<ViewModel4>();
        }

        [Test]
        public void can_find_views_from_both_application_and_bottle()
        {
            view1.ShouldNotBeNull();
            view2.ShouldNotBeNull();
            view3.ShouldNotBeNull();
            view4.ShouldNotBeNull();
        }

        [Test]
        public void has_the_correct_file_path_in_application()
        {
            view1.FilePath.ShouldBe(Folder.AppendPath(Application).AppendPath("View1.cshtml").ToFullPath());
        }


        [Test]
        public void view_path_of_application_file()
        {
            view1.ViewPath.ShouldBe("View1.cshtml");
        }


        [Test]
        public void relative_path_of_application_files()
        {
            view1.RelativePath().ShouldBe("View1.cshtml");
            view2.RelativePath().ShouldBe("Folder1/View2.cshtml");
        }


        [Test]
        public void relative_path_of_bottle_files()
        {
            view3.RelativePath().ShouldBe("View3.cshtml");
            view4.RelativePath().ShouldBe("Folder1/Folder2/View4.cshtml");
        }

        [Test]
        public void relative_directory_path_of_application_file()
        {
            view1.RelativeDirectoryPath().ShouldBe("");
            view2.RelativeDirectoryPath().ShouldBe("Folder1");
        }

        [Test]
        public void relative_directory_path_of_bottle_file()
        {
            view3.RelativeDirectoryPath().ShouldBe("");
            view4.RelativeDirectoryPath().ShouldBe("Folder1/Folder2");
        }

        [Test]
        public void name()
        {
            view1.Name().ShouldBe("View1");
            view2.Name().ShouldBe("View2");
            view3.Name().ShouldBe("View3");
            view4.Name().ShouldBe("View4");
        }


        [Test]
        public void namespace_from_application_view()
        {
            view1.Namespace.ShouldBe("FubuMVC.IntegrationTesting");
            view2.Namespace.ShouldBe("FubuMVC.IntegrationTesting.Folder1");
        }
    }
}