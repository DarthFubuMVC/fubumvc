using System;
using System.Reflection;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class ReflectionExtensionsTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ValueOrDefault_should_return_null_if_the_expression_results_in_null()
        {
            var test = new TestObject
            {
                Child = new TestObject()
            };

            test.ValueOrDefault(t => t.Child.Child).ShouldBeNull();
        }

        [Test]
        public void ValueOrDefault_should_return_null_if_the_root_is_null()
        {
            const TestObject nullTest = null;

            nullTest.ValueOrDefault(t => t.Child).ShouldBeNull();
        }

        [Test]
        public void
            ValueOrDefault_should_return_the_default_value_of_the_type_if_the_return_type_is_not_nullable_and_there_value_could_not_be_retrieved
            ()
        {
            var test = new TestObject
            {
                Child = new TestObject()
            };

            test.ValueOrDefault(t => t.Child.Child.Value).ShouldEqual(0);
        }

        [Test]
        public void ValueOrDefault_should_return_the_result_of_the_expression_if_there_are_no_nulls()
        {
            var test = new TestObject
            {
                Child = new TestObject
                {
                    Child = new TestObject()
                }
            };

            test.ValueOrDefault(t => t.Child.Child).ShouldNotBeNull();
        }

        [Test]
        public void get_attribute_on_method()
        {
            ReflectionHelper.GetMethod<AttributeClass>(x => x.GetName())
                .GetAttribute<DescriptionAttribute>().Description.ShouldEqual("att value");
        }

        [Test]
        public void has_attribute_on_method()
        {
            MethodInfo method = ReflectionHelper.GetMethod<AttributeClass>(x => x.GetName());
            method.HasAttribute<DescriptionAttribute>().ShouldBeTrue();
            method.HasAttribute<SetUpAttribute>().ShouldBeFalse();
        }

        [Test]
        public void for_attribute_negative_case_when_the_attribute_does_not_exist()
        {
            ReflectionHelper.GetProperty<AttributeClass>(x => x.Name).ForAttribute<FakeAttribute>(att =>
            {
                Assert.Fail("Should not execute this Action");
            });
        }

        [Test]
        public void for_attribute_positive_case_when_the_attribute_does_exist()
        {
            bool gotIntoAction = false;

            ReflectionHelper.GetProperty<AttributeClass>(x => x.Number).ForAttribute<FakeAttribute>(att =>
            {
                att.Name.ShouldEqual("number");
                gotIntoAction = true;
            });

            gotIntoAction.ShouldBeTrue();
        }

        [Test]
        public void get_attribute_from_accessor()
        {
            ReflectionHelper.GetAccessor<AttributeClass>(x => x.Number).GetAttribute<FakeAttribute>()
                .Name.ShouldEqual("number");
        }

        [Test]
        public void has_attribute_from_accessor()
        {
            ReflectionHelper.GetAccessor<AttributeClass>(x => x.Number)
                .HasAttribute<FakeAttribute>()
                .ShouldBeTrue();

            ReflectionHelper.GetAccessor<AttributeClass>(x => x.Name)
                .HasAttribute<FakeAttribute>()
                .ShouldBeFalse();
        }

        [Test]
        public void for_attribute_negative_case_when_the_attribute_does_not_exist_by_accessor()
        {
            ReflectionHelper.GetAccessor<AttributeClass>(x => x.Name).ForAttribute<FakeAttribute>(att =>
            {
                Assert.Fail("Should not execute this Action");
            });
        }

        [Test]
        public void for_attribute_positive_case_when_the_attribute_does_exist_by_accessor()
        {
            bool gotIntoAction = false;

            ReflectionHelper.GetAccessor<AttributeClass>(x => x.Number).ForAttribute<FakeAttribute>(att =>
            {
                att.Name.ShouldEqual("number");
                gotIntoAction = true;
            });

            gotIntoAction.ShouldBeTrue();
        }

    }

    public class AttributeClass
    {
        [Description("att value")]
        public string GetName() {
            return "Jeremy"; }

        [Fake("number")]
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public class FakeAttribute : Attribute
    {
        private readonly string _name;

        public FakeAttribute(string name)
        {
            _name = name;
        }

        public string Name { get { return _name; } }
    }
}