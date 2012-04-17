using System;
using System.Collections.Generic;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Ajax
{
    [TestFixture]
    public class AjaxContinuationWriterTester : InteractionContext<AjaxContinuationWriter<AjaxContinuation>>
    {
        private AjaxContinuation theContinuation;
        private Dictionary<string, object> theDictionary;

        protected override void beforeEach()
        {
            theContinuation = MockFor<AjaxContinuation>();

            theDictionary = new Dictionary<string, object>();
            theContinuation.Stub(x => x.ToDictionary()).Return(theDictionary);



            ClassUnderTest.Write(MimeType.Json.Value, theContinuation);
        }

        [Test]
        public void should_write_the_dictionary_of_the_continuation_to_json()
        {
            MockFor<IJsonWriter>().AssertWasCalled(x => x.Write(theDictionary, MimeType.Json.Value));
        }
    }
}