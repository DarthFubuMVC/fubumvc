using System.Linq;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Templates;
using FubuMVC.IntegrationTesting.Views;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.UI.Templates
{
    /*
     * TODO's
     * 1.) Prove that you can use an Endpoint and/or Actionless View
     * 2.) TemplateGraph construction
     * 3.) Write template file one at a time
     * 4.) Write all templates
     * 5.) Title of the template chain
     * 6.) Templates that are not generated from a view
     */

    [TestFixture]
    public class TemplateGenerationIntegration_Tests : ViewIntegrationContext
    {
        public TemplateGenerationIntegration_Tests()
        {
            SparkView<ViewModel1>("View1").Write("View1");
            SparkView<ViewModel2>("View2").Write("View2");
            SparkView<ViewModel3>("View3").Write("View3");

            SparkView<Template1>("Template1").Write(@"
Template1
The culture is '!{this.Culture}'
");

                        SparkView<Template2>("Template2").Write(@"
Template2
The culture is '!{this.Culture}'
");

                        SparkView<Template3>("Template3").Write(@"
Template3
The culture is '!{this.Culture}'
");
        }




        [Test]
        public void should_find_all_of_the_templates_and_only_the_templates()
        {
            var templates = Services.GetInstance<TemplateGraph>();
            templates.Templates().Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Template1", "Template2", "Template3");
        }
    }

    public class ViewModel1{}
    public class ViewModel2{}
    public class ViewModel3{}

    public class Template1 : Template
    {
        public string Culture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name;
            }
        }
    }
    public class Template2: Template1{}
    public class Template3: Template1{}


}