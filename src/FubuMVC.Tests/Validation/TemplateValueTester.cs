using System.Collections.Generic;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class TemplateValueTester
    {
        [Fact]
        public void equals_other_value()
        {
            var theTemplate = TemplateValue.For("Test", "123");
            theTemplate.ShouldBe(TemplateValue.For("Test", "123"));
        }

        [Fact]
        public void does_not_equal_value()
        {
            var theTemplate = TemplateValue.For("Test", "123");
            theTemplate.ShouldNotBe(TemplateValue.For("Test2", "1234"));
        }
    }

    
    public class TemplateValueCollectionTester
    {
        [Fact]
        public void gets_the_values()
        {
            var values = new List<TemplateValue>
            {
                TemplateValue.For("FirstName", "Joel"),
                TemplateValue.For("LastName", "Arnold")
            };

            var collection = new TemplateValueCollection(values);
            collection.ShouldHaveTheSameElementsAs(values);
        }

        [Fact]
        public void gets_the_values_from_the_static_builder()
        {
            var v1 = TemplateValue.For("FirstName", "Joel");
            var v2 = TemplateValue.For("LastName", "Arnold");

            var collection = TemplateValueCollection.For(v1, v2);
            collection.ShouldHaveTheSameElementsAs(v1, v2);
        }

        [Fact]
        public void adds_the_value()
        {
            var v1 = TemplateValue.For("FirstName", "Joel");
            var v2 = TemplateValue.For("LastName", "Arnold");

            var collection = new TemplateValueCollection();
            collection.Add(v1);
            collection.Add(v2);

            collection.ShouldHaveTheSameElementsAs(v1, v2);
        }

        [Fact]
        public void only_adds_the_value_once()
        {
            var v1 = TemplateValue.For("FirstName", "Joel");

            var collection = new TemplateValueCollection();
            collection.Add(v1);
            collection.Add(v1);

            collection.ShouldHaveTheSameElementsAs(v1);
        }

        [Fact]
        public void equality_check()
        {
            var v1 = TemplateValue.For("FirstName", "Joel");
            var v2 = TemplateValue.For("LastName", "Arnold");

            var collection1 = TemplateValueCollection.For(v1, v2);
            var collection2 = TemplateValueCollection.For(v1, v2);

            collection1.ShouldBe(collection2);
        }

        [Fact]
        public void equality_check_negative()
        {
            var v1 = TemplateValue.For("FirstName", "Joel");
            var v2 = TemplateValue.For("LastName", "Arnold");

            var collection1 = TemplateValueCollection.For(v1, v2);
            var collection2 = TemplateValueCollection.For(v1);

            collection1.ShouldNotBe(collection2);
        }

        [Fact]
        public void builds_the_dictionary()
        {
            var v1 = TemplateValue.For("FirstName", "Joel");
            var v2 = TemplateValue.For("LastName", "Arnold");

            var values = TemplateValueCollection.For(v1, v2).ToDictionary();
            values["FirstName"].ShouldBe("Joel");
            values["LastName"].ShouldBe("Arnold");
        }
    }

    
    public class TemplateTester
    {
        [Fact]
        public void renders_the_template()
        {
            var theValue = TemplateValue.For("field", "FirstName");
            var theTemplate = new Template(StringToken.FromKeyString("Test", "{field} is required"), theValue);

            theTemplate.Render().ShouldBe("FirstName is required");
        }

        [Fact]
        public void equality_check()
        {
            var theValue = TemplateValue.For("field", "FirstName");
            var template1 = new Template(StringToken.FromKeyString("Test", "{field} is required"), theValue);

            var template2 = new Template(StringToken.FromKeyString("Test", "{field} is required"));
            template2.Values.Add(theValue);

            template1.ShouldBe(template2);
        }

        [Fact]
        public void equality_check_negative()
        {
            var theValue = TemplateValue.For("field", "FirstName");
            var template1 = new Template(StringToken.FromKeyString("Test", "{field} is required"), theValue);

            var template2 = new Template(StringToken.FromKeyString("Test2", "{field} is invalid"));
            template2.Values.Add(theValue);

            template1.ShouldNotBe(template2);

            var template3 = new Template(StringToken.FromKeyString("Test3", "{field} is required"));
            template1.ShouldNotBe(template3);
        }
    }
}