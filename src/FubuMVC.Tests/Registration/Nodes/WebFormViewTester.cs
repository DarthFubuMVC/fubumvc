using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using NUnit.Framework;
using FubuMVC.Core.View.WebForms;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class WebFormViewTester
    {
        [Test]
        public void create_from_view_type()
        {
            var view = new WebFormView(typeof (FakeWebView));
            view.ViewName.ShouldEqual(typeof (FakeWebView).ToVirtualPath());
            view.InputType.ShouldEqual(typeof (InputModel));

            view.As<IMayHaveInputType>().InputType().ShouldEqual(typeof (InputModel));
        }
    }

    public class FakeWebView : FubuPage<InputModel>{}
}