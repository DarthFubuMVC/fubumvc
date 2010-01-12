using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View.WebForms;
using FubuMVC.Tests.View.FakeViews;
using NUnit.Framework;

namespace FubuMVC.Tests.View.WebForms
{
    [TestFixture]
    public class WebFormViewTokenTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            type = typeof (View4);
            token = new WebFormViewToken(type);
        }

        #endregion

        private Type type;
        private WebFormViewToken token;

        [Test]
        public void Name()
        {
            token.Name.ShouldEqual("View4");
        }

        [Test]
        public void Namespace()
        {
            token.Namespace.ShouldEqual(type.Namespace);
        }

        [Test]
        public void OutputType()
        {
            token.ViewModelType.ShouldEqual(typeof (ViewModel4));
        }

        [Test]
        public void ToBehavioralNode()
        {
            token.ToBehavioralNode().ShouldBeOfType<WebFormView>()
                .ViewName.ShouldEqual("~/View/View4.aspx");
        }
    }
}