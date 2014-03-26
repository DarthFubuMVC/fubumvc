using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Scenarios;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class templates_html_creation_specs
    {
        private ConventionTarget theTarget;
        private ElementGenerator<ConventionTarget> theGenerator;

        [SetUp]
        public void SetUp()
        {
            theTarget = new ConventionTarget();

            theGenerator = HtmlElementScenario<ConventionTarget>.For(x =>
            {
                x.Model = theTarget;
            });
        }

        [Test]
        public void label_is_just_the_same()
        {
            theGenerator.LabelFor(x => x.Name, profile: ElementConstants.Templates)
                .ToString().ShouldEqual("<label for=\"Name\">Name</label>");
        }

        [Test]
        public void default_display_is_just_a_span()
        {
            theGenerator.DisplayFor(x => x.Name, profile: ElementConstants.Templates)
                .ToString().ShouldEqual("<span data-fld=\"Name\">{{Name}}</span>");
        }

        [Test]
        public void default_input_is_textbox()
        {
            theGenerator.InputFor(x => x.Name, profile: ElementConstants.Templates)
                .ToString().ShouldEqual("<input type=\"text\" name=\"Name\" value=\"{{Name}}\" data-fld=\"Name\" />");
        }
    }
}