using System;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using InMemoryRequestData = FubuCore.Binding.InMemoryRequestData;

namespace FubuMVC.Tests.Runtime
{
    public class Model{}

    [TestFixture]
    public class FubuSmartRequestTester
    {
        private InMemoryRequestData theData;
        private SmartRequest theRequest;
        private InMemoryFubuRequest theFubuRequest;
        private ObjectConverter objectConverter;
        private ConverterLibrary theConverterLibrary;

        [SetUp]
        public void SetUp()
        {
            theFubuRequest = new InMemoryFubuRequest();
            theData = new InMemoryRequestData();

            theConverterLibrary = new ConverterLibrary();
            objectConverter = new ObjectConverter(null, theConverterLibrary);
            theRequest = new FubuSmartRequest(theData, objectConverter, theFubuRequest);
        }

        [Test]
        public void if_converter_cannot_handle_existing_value_type_use_fuburequest()
        {
            var model = new Model();
            theFubuRequest.Set(model);

            theRequest.Value(typeof (Model), "anything").ShouldBeTheSameAs(model);
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
            theConverterLibrary.RegisterConverter<Blob>(b => new Blob());
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