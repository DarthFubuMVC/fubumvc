using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using FubuMVC.Tests.View.FakeViews;
using NUnit.Framework;

namespace FubuMVC.Tests.View.WebForms
{
    [TestFixture]
    public class WebFormViewFacilityTester
    {
        [SetUp]
        public void SetUp()
        {
            var pool = new TypePool(null){
                ShouldScanAssemblies = true
            };
            pool.AddAssembly(Assembly.GetExecutingAssembly());

            views = new WebFormViewFacility().FindViews(pool, new BehaviorGraph());
        }

        private IEnumerable<IViewToken> views;

        [Test]
        public void IsWebFormView_for_fubu_page()
        {
            WebFormViewFacility.IsWebFormView(typeof (View4)).ShouldBeTrue();
            WebFormViewFacility.IsWebFormView(typeof (A)).ShouldBeTrue();

            WebFormViewFacility.IsWebFormView(GetType()).ShouldBeFalse();
        }

        [Test]
        public void is_web_form_control()
        {
            WebFormViewFacility.IsWebFormControl(typeof(WebFormControl)).ShouldBeTrue();
            WebFormViewFacility.IsWebFormView(GetType()).ShouldBeFalse();
        }


        [Test]
        public void picks_up_all_aspx_views()
        {
            views.Count().ShouldBeGreaterThan(0);

            views.Each(x => x.ShouldBeOfType<WebFormViewToken>().ViewType.CanBeCastTo<TemplateControl>().ShouldBeTrue());
        }

        [Test]
        public void create_view_node_for_a_web_forms_page()
        {
            new WebFormViewFacility().CreateViewNode(typeof (WebPage))
                .ShouldBeOfType<WebFormView>().ViewName.ShouldEqual(typeof(WebPage).ToVirtualPath());
        }

        [Test]
        public void create_view_node_for_a_web_forms_control()
        {
            new WebFormViewFacility().CreateViewNode(typeof (WebFormControl))
                .ShouldBeOfType<WebFormView>().ViewName.ShouldEqual(typeof (WebFormControl).ToVirtualPath());
        }

        [Test]
        public void create_view_node_for_a_not_web_forms_type_returns_null()
        {
            new WebFormViewFacility().CreateViewNode(GetType()).ShouldBeNull();
        }
    }

    public class WebFormControl : UserControl{}
    public class WebPage : FubuPage{}
}