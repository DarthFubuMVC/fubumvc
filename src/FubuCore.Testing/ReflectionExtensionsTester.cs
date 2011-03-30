using System;
using System.Reflection;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore.Testing;

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
        public void has_attribute_on_property()
        {
            var property = ReflectionHelper.GetProperty<BaseAttributeClass>(x => x.OnBase);
            property.HasAttribute<FakeAttribute>().ShouldBeTrue();
        }

        [Test]
        public void has_attribute_on_property_from_base_class()
        {
            var property = ReflectionHelper.GetProperty<DerivedAttributeClass>(x => x.OnBase);
            property.HasAttribute<FakeAttribute>().ShouldBeTrue();
        }

        [Test]
        public void has_attribute_on_property_from_base_class_but_overridden_in_derived_class()
        {
            var property = ReflectionHelper.GetProperty<DerivedAttributeClass>(x => x.OnBaseForOverride);
            property.HasAttribute<FakeAttribute>().ShouldBeTrue();
        }

        [Test]
        public void has_attribute_on_derived_property_but_not_base_cannot_be_found()
        {
            var property = ReflectionHelper.GetProperty<DerivedAttributeClass>(x => x.NotOnBase);
            property.HasAttribute<FakeAttribute>().ShouldBeFalse();
        }


        [Test]
        public void get_many_attributes()
        {
            ReflectionHelper.GetAccessor<AttributeClass>(x => x.Name).GetAllAttributes<Multiple>().ShouldHaveCount(3);
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

        [Multiple, Multiple, Multiple]
        public string Name { get; set; }
    }

    public class DerivedAttributeClass : BaseAttributeClass
    {
        public override string OnBaseForOverride
        {
            get { return "X"; } set { base.OnBaseForOverride = value; }
        }
        [Fake("baz")]
        public override string NotOnBase
        {
            get { return "X"; }
            set { base.OnBaseForOverride = value; }
        }
    }

    public class BaseAttributeClass
    {
        [Fake("bar")]
        public virtual string OnBaseForOverride { get; set; }
        [Fake("foo")]
        public virtual string OnBase { get; set; }

        public virtual string NotOnBase { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class Multiple : Attribute{}

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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