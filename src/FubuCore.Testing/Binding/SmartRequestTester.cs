using System;
using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    public class Model{}

    [TestFixture]
    public class SmartRequestTester
    {
        private InMemoryRequestData theData;
        private SmartRequest theRequest;
        private ObjectConverter objectConverter;

        [SetUp]
        public void SetUp()
        {
            theData = new InMemoryRequestData();
            objectConverter = new ObjectConverter();
            theRequest = new SmartRequest(theData, objectConverter);
        }


        [Test]
        public void if_value_is_null_return_null_from_Value()
        {
            theData["blob"] = null;
            theRequest.Value<Blob>("blob").ShouldBeNull();
        }

        [Test]
        public void do_not_convert_the_type_if_it_is_already_in_the_correct_type()
        {
            objectConverter.RegisterConverter<Blob>(b => new Blob());
            var theBlob = new Blob();
            theData["blob"] = theBlob;
            theRequest.Value<Blob>("blob").ShouldBeTheSameAs(theBlob);
        }

        [Test]
        public void convert_the_type_if_it_necessary()
        {
            theData["int"] = "5";
            theRequest.Value<int>("int").ShouldEqual(5);
            theRequest.Value(typeof(int), "int").ShouldEqual(5);
        }

        [Test]
        public void missing_value_with_continuation_does_nothing()
        {
            theRequest.Value<int>("number", i =>
            {
                Assert.Fail("I should not have been called");
            }).ShouldBeFalse();
        }

        [Test]
        public void found_value_with_continuation()
        {
            var action = MockRepository.GenerateMock<Action<int>>();
            theData["int"] = "5";

            theRequest.Value<int>("int", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(5));
        }
    }

    public class Blob
    {
        
    }
}