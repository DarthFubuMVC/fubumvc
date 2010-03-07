using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class DefaultHtmlConventionsTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        private ElementRequest For(Expression<Func<AddressViewModel, object>> expression)
        {
            return new ElementRequest(new AddressViewModel(), expression.ToAccessor(), null, new Stringifier());
        }

        [Test]
        public void add_element_name_to_select()
        {
            var select = new SelectTag();

            ElementRequest request = For(x => x.Address.City);
            request.ElementId = "AddressCity";

            DefaultHtmlConventions.AddElementName(request, select);

            select.Attr("name").ShouldEqual("AddressCity");
        }

        [Test]
        public void add_element_name_to_textbox()
        {
            var tag = new TextboxTag();
            ElementRequest request = For(x => x.Address.City);
            request.ElementId = "AddressCity";

            DefaultHtmlConventions.AddElementName(request, tag);

            tag.Attr("name").ShouldEqual("AddressCity");
        }

        [Test]
        public void do_not_add_element_name_to_span()
        {
            var span = new HtmlTag("span");

            ElementRequest request = For(x => x.Address.City);
            request.ElementId = "AddressCity";

            DefaultHtmlConventions.AddElementName(request, span);

            span.HasAttr("name").ShouldBeFalse();
        }
    }
}