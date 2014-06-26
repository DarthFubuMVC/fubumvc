using System.Linq;
using FubuMVC.Core;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.LayoutAttachment
{
    [TestFixture, Explicit]
    public class Explicit_declaration_of_master_layout_spark : ViewIntegrationContext
    {
        public Explicit_declaration_of_master_layout_spark()
        {
            SparkView("Shared/Application").Write("Application content");
            SparkView("Shared/Special").Write(@"
<use master='none' />



Special content");

            SparkView<MyViewModel>("View1").Write("<use master='Special'/>");
            SparkView<ViewModel2>("Folder1/View2");
            SparkView<ViewModel3>("Folder1/Folder2/View3");
            SparkView<ViewModel4>("Folder1/Folder2/View4");

            
        }

        public class MyViewModelRegistry : FubuRegistry
        {
            public MyViewModelRegistry()
            {
                Actions.IncludeType<MyViewModelEndpoint>();
            }
        }

        [Test]
        public void use_the_explicit_master_page_if_it_exists()
        {
            Scenario.Get.Input<MyViewModel>();

            Scenario.ContentShouldContain("Special content");
            Scenario.ContentShouldNotContain("Application content");
        }


        [Test]
        public void use_the_explicit_master_name_if_it_exists()
        {
            var view1 = Views.Templates<SparkTemplate>().FirstOrDefault(x => x.Name() == "View1");

            view1.Master.ShouldNotBeNull();
            view1.Master.Name().ShouldEqual("Special");

            view1.Master.Master.ShouldBeNull();
        }
    }

    public class MyViewModel{}

    public class MyViewModelEndpoint
    {
        public MyViewModel get_my_view_model(MyViewModel input)
        {
            return input;
        }
    }
}