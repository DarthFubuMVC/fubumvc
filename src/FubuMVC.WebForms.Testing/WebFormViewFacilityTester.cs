using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class WebFormViewFacilityTester
    {
        [SetUp]
        public void SetUp()
        {
            var pool = new TypePool(null);
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
        public void do_not_consider_ascx_files_as_views_when_doing_view_attachment()
        {
            WebFormViewFacility.IsWebFormView(typeof(WebFormControlMarkedAsFubuPage)).ShouldBeFalse();
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
    }

    public class WebFormControl : UserControl {}
    public class WebPage : FubuPage{}
    public class WebFormControlMarkedAsFubuPage : UserControl, IFubuPage {
        public string ElementPrefix
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IServiceLocator ServiceLocator
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IUrlRegistry Urls
        {
            get { throw new NotImplementedException(); }
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }

        public T GetNew<T>()
        {
            throw new NotImplementedException();
        }

        public void Write(object content)
        {
            throw new NotImplementedException();
        }
    }

}