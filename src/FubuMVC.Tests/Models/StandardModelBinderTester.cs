using System;
using System.Linq;
using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class StandardModelBinderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            registry = new ValueConverterRegistry(new IConverterFamily[0]);
            data = new InMemoryRequestData();
            binder = new StandardModelBinder(registry, new TypeDescriptorRegistry());
            locator = MockRepository.GenerateMock<IServiceLocator>();
            result = null;
        }

        #endregion

        private ValueConverterRegistry registry;
        private InMemoryRequestData data;
        private StandardModelBinder binder;
        private IServiceLocator locator;
        private BindResult result;

        private BindResult theResult
        {
            get
            {
                if (result == null)
                {
                    result = binder.Bind(typeof (Turkey), data);
                }

                return result;
            }
        }

        private Turkey theResultingObject { get { return theResult.Value.As<Turkey>(); } }

        private class Turkey
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int? NullableInt { get; set; }
            public bool Alive { get; set; }
            public DateTime BirthDate { get; set; }
            public Guid Id { get; set; }
            public bool X_Requested_With { get; set;}
        }

        [Test]
        public void
            Checkbox_handling__if_the_property_type_is_boolean_and_the_value_does_not_equal_the_name_and_isnt_a_recognizeable_boolean_a_problem_should_be_attached
            ()
        {
            data["Alive"] = "BOGUS";
            theResult.Problems.Count.ShouldEqual(1);

            ConvertProblem problem = theResult.Problems.First();

            problem.Property.Name.ShouldEqual("Alive");
        }

        [Test]
        public void
            Checkbox_handling__if_the_property_type_is_boolean_and_the_value_equals_the_name_then_set_the_property_to_true
            ()
        {
            data["Alive"] = "Alive";

            theResultingObject.Alive.ShouldBeTrue();
        }

        [Test]
        public void create_and_populate_should_convert_between_types()
        {
            data["Age"] = "12";
            data["Alive"] = "True";
            data["BirthDate"] = "01-JUN-2008";

            theResultingObject.Age.ShouldEqual(12);
            theResultingObject.Alive.ShouldBeTrue();
            theResultingObject.BirthDate.ShouldEqual(new DateTime(2008, 06, 01));
        }

        [Test]
        public void should_use_alternate_underscore_naming_if_primary_fails()
        {
            data["X-Requested-With"] = "True";

            theResultingObject.X_Requested_With.ShouldBeTrue();
        }

        [Test, Ignore("Removed requirement for case-insensitivity. May add back later")]
        public void
            create_and_populate_should_create_new_object_and_set_all_property_values_present_in_dictionary_regardless_of_key_casing
            ()
        {
            Assert.Fail("Do.");
            //var dict = new Dictionary<string, object> { { "nAme", "Sally" }, { "AGE", 12 } };

            //var item = new DictionaryConverter().ConvertFrom<Turkey>(dict, out _problems);
            //item.Name.ShouldEqual("Sally");
            //item.Age.ShouldEqual(12);
        }

        [Test]
        public void create_and_populate_should_not_throw_exception_during_type_conversion_and_return_a_meaningful_error()
        {
            data["Age"] = "abc";
            theResultingObject.Age.ShouldEqual(default(int));
            theResult.Problems.Count.ShouldEqual(1);

            ConvertProblem problem = theResult.Problems.First();
            problem.Exception.InnerException.ShouldBeOfType<FormatException>();
            problem.Item.ShouldBeTheSameAs(theResultingObject);
            problem.Property.Name.ShouldEqual("Age");
            problem.Value.ShouldEqual("abc");
        }

        [Test]
        public void does_not_match_class_without_no_arg_ctor()
        {
            binder.Matches(typeof (ClassWithoutNoArgCtor)).ShouldBeFalse();
        }

        [Test]
        public void matches_class_with_no_arg_ctor()
        {
            binder.Matches(typeof (ClassWithNoArgCtor)).ShouldBeTrue();
        }

        [Test]
        public void no_errors_on_clean_transfer_of_valid_properties_to_object()
        {
            data["Name"] = "Boris";
            data["Age"] = "2";

            theResult.Problems.Count.ShouldEqual(0);
        }

        [Test]
        public void populate_extra_values_in_dictionary_are_ignored()
        {
            data["xyzzy"] = "foo";

            theResult.Problems.Count.ShouldEqual(0);

            theResultingObject.Name.ShouldBeNull();
            theResultingObject.Age.ShouldEqual(0);
        }

        [Test]
        public void populate_should_not_change_property_values_not_found_in_the_dictionary()
        {
            var item = new Turkey
            {
                Name = "Smith"
            };
            data["Age"] = 9;

            binder.Populate(item, data);

            item.Name.ShouldEqual("Smith");
            item.Age.ShouldEqual(9);
        }

        [Test]
        public void populate_should_set_all_property_values_present_in_dictionary()
        {
            data["Name"] = "Boris";
            data["Age"] = "2";

            theResultingObject.Name.ShouldEqual("Boris");
            theResultingObject.Age.ShouldEqual(2);
        }

        [Test, Ignore("Removed requirement for case-insensitivity. May add back later")]
        public void populate_should_set_all_property_values_present_in_dictionary_regardless_of_key_casing()
        {
            Assert.Fail("Do.");
            //var item = new Turkey();

            //var dict = new Dictionary<string, object> { { "nAme", "Smith" }, { "AGE", 9 } };

            //new DictionaryConverter().Populate(dict, item, out _problems);
            //item.Name.ShouldEqual("Smith");
            //item.Age.ShouldEqual(9);
        }


        [Test]
        public void Read_a_boolean_type_that_is_false()
        {
            data["Alive"] = "";

            theResultingObject.Alive.ShouldBeFalse();
        }

        [Test]
        public void Read_a_boolean_type_that_is_true()
        {
            data["Alive"] = "true";
            theResultingObject.Alive.ShouldBeTrue();
        }

        [Test]
        public void Read_a_Nullable_value_type()
        {
            data["NullableInt"] = "8";
            theResultingObject.NullableInt.ShouldEqual(8);
        }

        [Test]
        public void Read_a_Nullable_value_type_empty_string_as_null()
        {
            data["NullableInt"] = string.Empty;
            theResultingObject.NullableInt.ShouldBeNull();

            theResult.Problems.Count.ShouldEqual(0);
        }

        [Test]
        public void should_convert_from_string_to_guid()
        {
            Guid guid = Guid.NewGuid();
            data["Id"] = guid.ToString();

            theResultingObject.Id.ShouldEqual(guid);
        }
    }

    public class ClassWithoutNoArgCtor
    {
        public ClassWithoutNoArgCtor(int something)
        {
        }
    }

    public class ClassWithNoArgCtor
    {
    }
}