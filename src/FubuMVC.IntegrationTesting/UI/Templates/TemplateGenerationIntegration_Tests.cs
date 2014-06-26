using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Templates;
using FubuMVC.IntegrationTesting.Views;
using FubuTestingSupport;
using HtmlTags;
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
        private readonly FileSystem Files = new FileSystem();

        public TemplateGenerationIntegration_Tests()
        {
            SparkView<NonTemplate1>("NonTemplate1").Write("View1");
            SparkView<NonTemplate2>("NonTemplate2").Write("View2");
            SparkView<NonTemplate3>("NonTemplate3").Write("View3");

            SparkView<Template1>("Template1").Write(@"
Template1
The culture is '!{Model.Culture}'
");

            SparkView<Template2>("Template2").Write(@"
!{Model.Title}
The culture is '!{Model.Culture}'
");


        }

        private TemplateGraph templates
        {
            get
            {
                return Services.GetInstance<TemplateGraph>();
            }
        }

        public class TemplateRegistry : FubuRegistry
        {
            public TemplateRegistry()
            {
                Actions.IncludeType<TemplatingEndpoint>();

                AlterSettings<AssetSettings>(x => {
                    x.TemplateDestination = "public/templates";
                    x.TemplateCultures.Add("en-CA");
                    x.TemplateCultures.Add("fr-FR");
                });
            }
        }

        [Test]
        public void graph_uses_asset_settings_to_get_the_template_destination()
        {
            templates.TemplateDirectory.ShouldEndWith("public/templates");
            Path.IsPathRooted(templates.TemplateDirectory).ShouldBeTrue();
        }


        [Test]
        public void should_find_all_of_the_templates_and_only_the_templates()
        {
            templates.Templates().Select(x => x.Name).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("Template1", "Template2", "Template3");
        }

        [Test]
        public void the_path_of_written_templates_should_be_by_culture_and_name()
        {
            templates["Template1"].PathFor(new CultureInfo("es-ES"))
                .ShouldEndWith("public/templates/es-ES/Template1.htm");

            templates["Template1"].PathFor(new CultureInfo("fr-FR"))
                .ShouldEndWith("public/templates/fr-FR/Template1.htm");
        }

        [Test]
        public void find_a_template_by_file()
        {
            var template = templates["Template1"];
            templates[template.File].ShouldBeTheSameAs(template);
        }

        [Test]
        public void generate_the_text_for_a_single_template_in_an_actionless_view()
        {
            var template = templates["Template1"];
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");


            var text = template.GenerateTextForCurrentThreadCulture();
            text.ShouldContain("The culture is \"fr-FR\"");
            text.ShouldContain("Template1");
        }

        [Test]
        public void generate_the_text_for_a_view_with_endpoint()
        {
            var template = templates["Template2"];
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");


            var text = template.GenerateTextForCurrentThreadCulture();
            text.ShouldContain("The culture is \"fr-FR\"");
            text.ShouldContain("This is Template2 text from the Action");
        }

        [Test]
        public void generate_the_text_for_a_template_without_a_view()
        {
            var template = templates["Template3"];
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");


            var text = template.GenerateTextForCurrentThreadCulture();
            text.ShouldContain("This is Template3 with culture fr-FR");
        }

        [Test]
        public void write_a_single_template_from_actionless_view()
        {
            templates.Write("Template1", "en-CA");

            var path = templates["Template1"].PathFor(new CultureInfo("en-CA"));

            var text = Files.ReadStringFromFile(path);

            text.ShouldContain("The culture is \"en-CA\"");
            text.ShouldContain("Template1");
        }

        [Test]
        public void write_all()
        {
            templates.WriteAll();

            Files.FindFiles(templates.TemplateDirectory, FileSet.Deep("*.htm"))
                .Select(x => x.PathRelativeTo(templates.TemplateDirectory).Replace('\\', '/'))
                .OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("en-CA/Template1.htm", "en-CA/Template2.htm", "en-CA/Template3.htm", "en-US/Template1.htm", "en-US/Template2.htm", "en-US/Template3.htm", "fr-FR/Template1.htm", "fr-FR/Template2.htm", "fr-FR/Template3.htm");

        }
    }

    public class NonTemplate1{}
    public class NonTemplate2{}
    public class NonTemplate3{}

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
    public class Template2: Template
    {
        public string Title { get; set; }

        public string Culture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name;
            }
        }
    }

    public class Template3 : Template
    {
        public string Culture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name;
            }
        }
    }

    public class TemplatingEndpoint
    {
        public Template2 get_template2(Template2 model)
        {
            model.Title = "This is Template2 text from the Action";

            return model;
        }

        public HtmlTag get_template3(Template3 model)
        {
            return new HtmlTag("h1").Text("This is Template3 with culture " + Thread.CurrentThread.CurrentCulture.Name);
        }
    }
}