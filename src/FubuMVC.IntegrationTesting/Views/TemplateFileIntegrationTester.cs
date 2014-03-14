using FubuCore;
using FubuMVC.Core.View.Attachment;
using FubuMVC.Core.View.Model;
using FubuTestingSupport;
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
        
            InBottle("BottleA");
            RazorView<ViewModel3>("View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");
        }

        [SetUp]
        public void SetUp()
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
            view1.FilePath.ShouldEqual(Folder.AppendPath(Application).AppendPath("View1.cshtml").ToFullPath());
        }

        [Test]
        public void has_the_correct_file_path_in_bottle()
        {
            view3.FilePath.ShouldEqual(Folder.AppendPath("BottleA").AppendPath("View3.cshtml").ToFullPath());
            view4.FilePath.ShouldEqual(Folder.AppendPath("BottleA").AppendPath("Folder1/Folder2/View4.cshtml").ToFullPath());
        }

        [Test]
        public void root_path_in_application()
        {
            view1.RootPath.ShouldEqual(Folder.AppendPath(Application).ToFullPath());
        }

        [Test]
        public void root_path_in_bottle()
        {
            view3.RootPath.ShouldEqual(Folder.AppendPath("BottleA").ToFullPath());
        }

        [Test]
        public void origin_of_application_file()
        {
            view1.Origin.ShouldEqual("Application");
        }

        [Test]
        public void origin_of_Bottle_file()
        {
            view3.Origin.ShouldEqual("BottleA");
        }

        [Test]
        public void view_path_of_application_file()
        {
            view1.ViewPath.ShouldEqual("View1.cshtml");
        }

        [Test]
        public void view_path_of_bottle_file()
        {
            view3.ViewPath.ShouldEqual("_BottleA/View3.cshtml");
            view4.ViewPath.ShouldEqual("_BottleA/Folder1/Folder2/View4.cshtml");
        }

        [Test]
        public void relative_path_of_application_files()
        {
            view1.RelativePath().ShouldEqual("View1.cshtml");
            view2.RelativePath().ShouldEqual("Folder1/View2.cshtml");
        }


        [Test]
        public void relative_path_of_bottle_files()
        {
            view3.RelativePath().ShouldEqual("View3.cshtml");
            view4.RelativePath().ShouldEqual("Folder1/Folder2/View4.cshtml");
        }

        [Test]
        public void relative_directory_path_of_application_file()
        {
            view1.RelativeDirectoryPath().ShouldEqual("");
            view2.RelativeDirectoryPath().ShouldEqual("Folder1");
        }

        [Test]
        public void relative_directory_path_of_bottle_file()
        {
            view3.RelativeDirectoryPath().ShouldEqual("");
            view4.RelativeDirectoryPath().ShouldEqual("Folder1/Folder2");
        }

        [Test]
        public void name()
        {
            view1.Name().ShouldEqual("View1");
            view2.Name().ShouldEqual("View2");
            view3.Name().ShouldEqual("View3");
            view4.Name().ShouldEqual("View4");
        }

        [Test]
        public void from_host()
        {
            view1.FromHost().ShouldBeTrue();
            view2.FromHost().ShouldBeTrue();
            
            // these views are from a bottle
            view3.FromHost().ShouldBeFalse();
            view4.FromHost().ShouldBeFalse();
        }

        [Test]
        public void namespace_from_application_view()
        {
            view1.Namespace.ShouldEqual("FubuMVC.IntegrationTesting");
            view2.Namespace.ShouldEqual("FubuMVC.IntegrationTesting.Folder1");
        }

        [Test]
        public void full_name_of_application_file()
        {
            view1.FullName().ShouldEqual("FubuMVC.IntegrationTesting.View1");
            view2.FullName().ShouldEqual("FubuMVC.IntegrationTesting.Folder1.View2");
        }
    }
}