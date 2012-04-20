using System;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class WebFormViewTokenTester
    {
        [SetUp]
        public void SetUp()
        {
            type = typeof (View4);
            token = new WebFormViewToken(type);
        }

        private Type type;
        private WebFormViewToken token;

        [Test]
        public void Name()
        {
            token.ViewType.Name.ShouldEqual("View4");
        }

        [Test]
        public void Namespace()
        {
            token.ViewType.Namespace.ShouldEqual(type.Namespace);
        }

        [Test]
        public void OutputType()
        {
            token.ViewModel.ShouldEqual(typeof (ViewModel4));
        }

    }
}