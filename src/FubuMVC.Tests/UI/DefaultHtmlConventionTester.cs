using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Scenarios;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class DefaultHtmlConventionTester
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
        public void build_the_display()
        {
            theGenerator.DisplayFor(x => x.Name).ToString().ShouldEqual("<span id=\"Name\"></span>");

            theTarget.Name = "Shiner";

            theGenerator.DisplayFor(x => x.Name).ToString().ShouldEqual("<span id=\"Name\">Shiner</span>");
        }

        [Test]
        public void input_for_a_boolean()
        {
            // true
            theTarget.Passed = true;
            theGenerator.InputFor(x => x.Passed).ToString().ShouldEqual("<input type=\"checkbox\" checked=\"true\" name=\"Passed\" />");

            // false
            theTarget.Passed = false;
            theGenerator.InputFor(x => x.Passed).ToString().ShouldEqual("<input type=\"checkbox\" name=\"Passed\" />");
        }

        [Test]
        public void input_for_string()
        {
            theGenerator.InputFor(x => x.Name).ToString().ShouldEqual("<input type=\"text\" value=\"\" name=\"Name\" />");

            theTarget.Name = "Shiner";

            theGenerator.InputFor(x => x.Name).ToString().ShouldEqual("<input type=\"text\" value=\"Shiner\" name=\"Name\" />");
        }

        [Test]
        public void label()
        {
            theGenerator.LabelFor(x => x.Name).ToString().ShouldEqual("<label for=\"Name\">Name</label>");
            theGenerator.LabelFor(x => x.BigName).ToString().ShouldEqual("<label for=\"BigName\">Big Name</label>");
        }

    }
}