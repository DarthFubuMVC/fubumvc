using System;
using System.Collections.Generic;
using System.Web;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class BasicExtensionsTester
    {
        public class TestObject
        {
            public TestObject Child { get; set; }
            public int Value { get; set; }
            public bool Boolean { get; set; }
        }

        [Test]
        public void FirstValue()
        {
            var objects = new TestObject[] {new TestObject(), new TestObject(), new TestObject()};
            objects.FirstValue(x => x.Child).ShouldBeNull();

            var theChild = new TestObject();
            objects[1].Child = theChild;
            objects[2].Child = new TestObject();

            objects.FirstValue(x => x.Child).ShouldBeTheSameAs(theChild);
        }

        public void UrlEncode_should_encode_string()
        {
            string test = "encode test";

            test.UrlEncoded().ShouldEqual("encode%20test");
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void If_should_also_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".If(() => test.Boolean && test.Boolean);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void If_should_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Value = 1
            };

            "test".If(() => test.Value == 1);
        }

        [Test]
        public void If_should_return_empty_string_value()
        {
            var test = new TestObject
            {
                Boolean = false
            };

            "test".If(() => test.Boolean).ShouldEqual("");
        }

        [Test]
        public void If_should_return_original_value()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".If(() => test.Boolean).ShouldEqual("test");
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void IfNot_should_also_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".IfNot(() => test.Boolean && test.Boolean);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void IfNot_should_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Value = 1
            };

            "test".IfNot(() => test.Value == 1);
        }

        [Test]
        public void IfNot_should_return_empty_string_value()
        {
            var test = new TestObject
            {
                Boolean = false
            };

            "test".IfNot(() => test.Boolean).ShouldEqual("test");
        }

        [Test]
        public void IfNot_should_return_original_value()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".IfNot(() => test.Boolean).ShouldEqual("");
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
        public void AddMany_should_add_range()
        {
            var list = new List<string>();
            list.AddMany("test").ShouldHave(item=>item == "test");
        }

        [Test]
        public void Exists_should_return_if_evaluator_correspond()
        {
            var values = new[] {"test"};
            Func<string, bool> evaluator = x => x.StartsWith("te");
            values.Exists(evaluator).ShouldBeTrue();
        }

        [Test]
        public void Get_should_return_dictionary_value()
        {
            var dictionary = new Dictionary<string, string> {{"key", "value"}};
            dictionary.Get("key", "fake").ShouldEqual("value");
        }

        [Test]
        public void Get_should_return_default_value()
        {
            var dictionary = new Dictionary<string, string> { { "key", "value" } };
            dictionary.Get("non existant key", "default value").ShouldEqual("default value");
        }

        [Test]
        public void Get_should_return_value()
        {
            var dictionary = new Dictionary<string, string> { { "key", "value" } };
            dictionary.Get("key").ShouldEqual("value");
        }

        [Test]
        public void should_return_is_http_context_ajax_request()
        {
            var context = new HttpContext(new HttpRequest("foo.txt", "http://test", ""),
                                          new HttpResponse(Console.Out));
            context.IsAjaxRequest().ShouldBeFalse();
        }

        [Test]
        public void should_return_dictionary_ajax_request()
        {
            object value = null;
            IDictionary<string, object> requestInput = new Dictionary<string, object>{{"key", value}};
            requestInput.IsAjaxRequest().ShouldBeFalse();
        }

        [Test]
        public void should_return_is_nullable_of()
        {
            Type nullableOfInt = typeof (int?);
            nullableOfInt.IsNullableOf(typeof (int)).ShouldBeTrue();
        }

        [Test]
        public void ItererateFromZero_should_run_action_x_times()
        {
            Iterated iterated = MockRepository.GenerateStub<Iterated>();
            iterated.Stub(i => i.Action(Arg<int>.Is.LessThan(6))).Repeat.Times(6);
            6.IterateFromZero(iterated.Action);
            iterated.AssertWasCalled(i => i.Action(Arg<int>.Is.LessThan(6)), c => c.Repeat.Times(6));
        }

        [Test]
        public void ToBool_should_return_boolean_value()
        {
            "True".ToBool().ShouldBeTrue();
        }

        [Test]
        public void ToBool_should_return_false_for_empty_string()
        {
            "".ToBool().ShouldBeFalse();
        }

        [Test]
        public void ToFullUrl_should_return_full_url()
        {
            UrlContext.Stub("MyFakeDomain");
            "SomeUrl".ToFullUrl().ShouldEqual("MyFakeDomain/SomeUrl");
        }

        public class Iterated { public virtual void Action(int idx){}}
    }
}