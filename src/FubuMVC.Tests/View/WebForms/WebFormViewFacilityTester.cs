using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Registration;
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
            var pool = new TypePool();
            pool.AddAssembly(Assembly.GetExecutingAssembly());

            views = new WebFormViewFacility().FindViews(pool);
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
        public void picks_up_all_aspx_views()
        {
            views.Count().ShouldBeGreaterThan(0);

            views.Each(x => x.ShouldBeOfType<WebFormViewToken>().ViewType.CanBeCastTo<TemplateControl>().ShouldBeTrue());
        }
    }
}