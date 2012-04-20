using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.View.FakeViews;
using FubuMVC.WebForms;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Tests.View.WebForms
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