using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class ElementConventionsFubuPageExtensions_FullStackTester_DefaultConventions : FubuPageExtensionContext
    {
        [Test]
        public void input_for_default_conventions()
        {
            execute(page => {
                page.Model.Name = "Jeremy";
                return page.InputFor(x => x.Name);
            });

            theResult.ShouldEqual("<input type=\"text\" value=\"Jeremy\" name=\"Name\" />");
        }

        [Test]
        public void input_for_a_completely_different_model()
        {
            execute(page => {
                var model = new DifferentThing {Order = 123};

                page.Get<IFubuRequest>().Set(model);

                return page.InputFor<DifferentThing>(x => x.Order);
            });

            theResult.ShouldEqual("<input type=\"text\" value=\"123\" name=\"Order\" />");
        }

        [Test]
        public void input_for_a_different_model()
        {
            execute(page => {
                var model = new ConventionTarget {Name = "Max"};

                return page.InputFor(model, x => x.Name);
            });

            theResult.ShouldEqual("<input type=\"text\" value=\"Max\" name=\"Name\" />");
        }


        [Test]
        public void label_for_expression()
        {
            execute(page => page.LabelFor(x => x.Name));
            theResult.ShouldEqual("<label for=\"Name\">Name</label>");

            execute(page => page.LabelFor(x => x.MaximumLengthProp));
            theResult.ShouldEqual("<label for=\"MaximumLengthProp\">Maximum Length Prop</label>");
        }

        [Test]
        public void label_for_a_different_thing()
        {
            execute(page => page.LabelFor<DifferentThing>(x => x.Order));

            theResult.ShouldEqual("<label for=\"Order\">Order</label>");
        }


        [Test]
        public void display_for_default_conventions()
        {
            execute(page =>
            {
                page.Model.Name = "Jeremy";
                return page.DisplayFor(x => x.Name);
            });

            theResult.ShouldEqual("<span id=\"Name\">Jeremy</span>");
        }

        [Test]
        public void display_for_a_completely_different_model()
        {
            execute(page =>
            {
                var model = new DifferentThing { Order = 123 };

                page.Get<IFubuRequest>().Set(model);

                return page.DisplayFor<DifferentThing>(x => x.Order);
            });

            theResult.ShouldEqual("<span id=\"Order\">123</span>");
        }

        [Test]
        public void display_for_a_different_model()
        {
            execute(page =>
            {
                var model = new ConventionTarget { Name = "Max" };

                return page.DisplayFor(model, x => x.Name);
            });

            theResult.ShouldEqual("<span id=\"Name\">Max</span>");
        }
    }

    public class DifferentThing
    {
        public int Order { get; set; }
    }
}