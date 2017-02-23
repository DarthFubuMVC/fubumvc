using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using Xunit;
using Shouldly;

namespace FubuMVC.IntegrationTesting.UI
{
    
    public class ElementConventionsFubuPageExtensions_FullStackTester_DefaultConventions : FubuPageExtensionContext
    {
        [Fact]
        public void input_for_default_conventions()
        {
            execute(page => {
                page.Model.Name = "Jeremy";
                return page.InputFor(x => x.Name);
            });

            theResult.ShouldBe("<input type=\"text\" value=\"Jeremy\" name=\"Name\" />");
        }

        [Fact]
        public void input_for_a_completely_different_model()
        {
            execute(page => {
                var model = new DifferentThing {Order = 123};

                page.Get<IFubuRequest>().Set(model);

                return page.InputFor<DifferentThing>(x => x.Order);
            });

            theResult.ShouldBe("<input type=\"text\" value=\"123\" name=\"Order\" />");
        }

        [Fact]
        public void input_for_a_different_model()
        {
            execute(page => {
                var model = new ConventionTarget {Name = "Max"};

                return page.InputFor(model, x => x.Name);
            });

            theResult.ShouldBe("<input type=\"text\" value=\"Max\" name=\"Name\" />");
        }


        [Fact]
        public void label_for_expression()
        {
            execute(page => page.LabelFor(x => x.Name));
            theResult.ShouldBe("<label for=\"Name\">en-US_Name</label>");

            execute(page => page.LabelFor(x => x.MaximumLengthProp));
            theResult.ShouldBe("<label for=\"MaximumLengthProp\">en-US_MaximumLengthProp</label>");
        }

        [Fact]
        public void label_for_a_different_thing()
        {
            execute(page => page.LabelFor<DifferentThing>(x => x.Order));

            theResult.ShouldBe("<label for=\"Order\">en-US_Order</label>");
        }


        [Fact]
        public void display_for_default_conventions()
        {
            execute(page =>
            {
                page.Model.Name = "Jeremy";
                return page.DisplayFor(x => x.Name);
            });

            theResult.ShouldBe("<span id=\"Name\">Jeremy</span>");
        }

        [Fact]
        public void display_for_a_completely_different_model()
        {
            execute(page =>
            {
                var model = new DifferentThing { Order = 123 };

                page.Get<IFubuRequest>().Set(model);

                return page.DisplayFor<DifferentThing>(x => x.Order);
            });

            theResult.ShouldBe("<span id=\"Order\">123</span>");
        }

        [Fact]
        public void display_for_a_different_model()
        {
            execute(page =>
            {
                var model = new ConventionTarget { Name = "Max" };

                return page.DisplayFor(model, x => x.Name);
            });

            theResult.ShouldBe("<span id=\"Name\">Max</span>");
        }
    }

    public class DifferentThing
    {
        public int Order { get; set; }
    }

    
    public class FubuPageExtensionContext : IDisposable
    {
        public FubuPageExtensionContext()
        {
            _server = FubuRuntime.Basic();
        }

        public void Dispose()
        {
            _server.Dispose();
        }


        protected string theResult = string.Empty;
        private FubuRuntime
            _server;

        public string BaseAddress
        {
            get { return _server.BaseAddress; }
        }


        protected void execute(Func<IFubuPage<ConventionTarget>, object> func)
        {
            ConventionEndpoint.Source = func;

            var response = _server.Scenario(_ =>
            {
                _.Get.Action<ConventionEndpoint> (x => x.get_result());
            });

            theResult = response.Body.ReadAsText();
        }
    }

}