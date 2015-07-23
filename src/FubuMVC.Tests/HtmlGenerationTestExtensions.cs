using Shouldly;
using HtmlTags;

namespace FubuMVC.Tests
{
    public static class HtmlGenerationTestExtensions
    {
        public static HtmlTag ShouldHaveTagName(this HtmlTag tag, string tagName)
        {
            tag.TagName().ShouldBe(tagName);

            return tag;
        }

        public static HtmlTag ShouldHaveClass(this HtmlTag tag, string className)
        {
            tag.HasClass(className).ShouldBeTrue();
            return tag;
        }

        public static HtmlTag ShouldNotHaveClass(this HtmlTag tag, string className)
        {
            tag.HasClass(className).ShouldBeFalse();
            return tag;
        }

        public static HtmlTag ShouldHaveAttribute(this HtmlTag tag, string key, string value)
        {
            tag.Attr(key).ShouldBe(value);
            return tag;
        }

        public static HtmlTag ShouldBeTextbox(this HtmlTag tag)
        {
            tag.ShouldHaveTagName("input");
            tag.ShouldHaveAttribute("type", "text");
            return tag;
        }

        public static HtmlTag ShouldBeSelect(this HtmlTag tag)
        {
            tag.ShouldHaveTagName("select");
            return tag;
        }

        //public static HtmlTag ShouldHaveElement(this HtmlTag node, string elementName, string requiredAttributeName, string requiredAttributeValue)
        //{
        //    var element = node.AllChildNodes().FirstOrDefault(e =>
        //    {
        //        if (e.Name != elementName) return false;
        //        var attribute = e.Attributes[requiredAttributeName];
        //        if (attribute == null) return false;
        //        return (attribute.Value == requiredAttributeValue);
        //    });
        //    if (element == null) Assert.Fail("Expected '{0}' to contain a {1} that has {2} = '{3}'.", node.OuterHtml, elementName, requiredAttributeName, requiredAttributeValue);
        //    return element;
        //}

        //public static HtmlTag ShouldHaveElementWithInnerText(this HtmlTag node, string elementName, string innerText)
        //{
        //    var element = node.AllChildNodes().FirstOrDefault(e => e.Name == elementName && e.InnerText.Trim() == innerText);
        //    if (element == null) Assert.Fail("Expected '{0}' to contain a {1} that has innerText = '{2}'.", node.OuterHtml, elementName, innerText);
        //    return element;
        //}

        //public static HtmlTag ShouldHaveElement(this HtmlTag node, string elementName)
        //{
        //    var element = node.AllChildNodes().FirstOrDefault(child => child.Name == elementName);
        //    if (element == null) Assert.Fail("Expected '{0}' to contain <{1}> element.", node.OuterHtml, elementName);
        //    return element;
        //}

        //public static HtmlTag ShouldBeElement(this HtmlTag node, string elementName)
        //{
        //    if (node.Name != elementName) Assert.Fail("Expected '{0}' to be <{1}> element.", node.OuterHtml, elementName);
        //    return node;
        //}

        //public static IEnumerable<HtmlTag> AllChildNodes(this HtmlTag node)
        //{
        //    foreach (var childNode in node.ChildNodes)
        //    {
        //        if (childNode.NodeType != HtmlTagType.Element) continue;
        //        yield return childNode;
        //        foreach (var child in childNode.AllChildNodes())
        //        {
        //            yield return child;
        //        }
        //    }
        //}

        //public static HtmlTag ShouldNotHaveElement(this HtmlTag node, Func<HtmlTag, bool> criteria)
        //{
        //    var element = node.AllChildNodes().FirstOrDefault(criteria);
        //    if (element != null) Assert.Fail("Expected '{0}' to not have an element with specified the criteria.", node.OuterHtml);
        //    return element;
        //}

        //public static HtmlTag ShouldNotHaveElement(this HtmlTag node, string elementName)
        //{
        //    var element = node.AllChildNodes().FirstOrDefault(child => child.Name == elementName);
        //    if (element != null) Assert.Fail("Unexpected '{0}' element.", elementName);
        //    return element;
        //}

        //public static HtmlTag ShouldHaveInnerText(this HtmlTag node, string innerText)
        //{
        //    node.InnerText.Trim().ShouldBe(innerText);
        //    return node;
        //}

        //public static HtmlTag ShouldNotHaveAttribute(this HtmlTag node, string attributeName)
        //{
        //    var attribute = node.Attributes[attributeName];
        //    if (attribute != null) Assert.Fail("Element has unexpected attribute '{0}' with value '{1}'", attribute.Name, attribute.Value);
        //    return node;
        //}

        //public static HtmlTag ShouldNotHaveAttributeValue(this HtmlTag node, string attributeName, string attributeValue)
        //{
        //    var attribute = node.Attributes[attributeName];
        //    if (attribute != null && attribute.Value == attributeValue) Assert.Fail("Element has unexpected attribute '{0}' with value '{1}'", attribute.Name, attribute.Value);
        //    return node;
        //}

        //public static HtmlTag ShouldNotHaveCssClass(this HtmlTag node, string className)
        //{
        //    var attribute = node.Attributes["class"];
        //    if (attribute != null)
        //    {
        //        attribute.Value.Split(' ').ShouldNotContain(className);
        //    }
        //    return node;
        //}


        //public static HtmlTag ShouldHaveCssClass(this HtmlTag node, string className)
        //{
        //    var attribute = node.Attributes["class"];

        //    if (attribute == null) Assert.Fail("Element does not have any CSS classes");
        //    attribute.Value.Split(' ').ShouldContain(className);
        //    return node;
        //}

        //public static HtmlAttribute ShouldHaveAttribute(this HtmlTag node, string attributeName)
        //{
        //    var attribute = node.Attributes[attributeName];
        //    if (attribute == null) Assert.Fail("Element does not have attribute '{0}'", attributeName);
        //    return attribute;
        //}


        //public static HtmlTag ShouldHaveAttributeValue(this HtmlTag node, string attributeName, string attributeValue)
        //{
        //    node.ShouldHaveAttribute(attributeName).Value.ShouldBe(attributeValue);
        //    return node;
        //}

        //public static HtmlTag ShouldHaveElement(this string s, string elementName)
        //{
        //    return s.ShouldRenderValidHtml().DocumentNode.ShouldHaveElement(elementName);
        //}

        //public static HtmlTag AsHtml(this string s)
        //{
        //    return s.ShouldRenderValidHtml().DocumentNode;
        //}

        //public static HtmlTag AsHtmlElement(this string s)
        //{
        //    var firstElement = s.AsHtml().FirstChild;
        //    firstElement.ShouldNotBeNull();
        //    return firstElement;
        //}

        //public static HtmlDocument ShouldRenderValidHtml(this string s)
        //{
        //    if (TestHelper.IsRunningInteractively())
        //    {
        //        Debug.WriteLine(s);
        //    }
        //    var doc = new HtmlDocument
        //    {
        //        OptionAutoCloseOnEnd = false,
        //        OptionCheckSyntax = true,
        //        OptionFixNestedTags = false
        //    };
        //    doc.LoadHtml(s);
        //    if (doc.ParseErrors.Count > 0)
        //    {
        //        string errors = null;
        //        foreach (var error in doc.ParseErrors)
        //        {
        //            var parseError = (HtmlParseError)error;
        //            errors += (String.IsNullOrEmpty(parseError.Reason) ? parseError.Code.ToString() : parseError.Reason) + Environment.NewLine;
        //        }
        //        Assert.Fail("Invalid HTML: {0}{1}{2}", s, Environment.NewLine, errors);
        //    }
        //    return doc;
        //}
    }
}