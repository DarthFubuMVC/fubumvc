using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class when_executing_a_content_plan : InteractionContext<ContentPlanExecutor>
    {
        private ProcessContentAction theContinuation;
        private AssetFile[] theFiles;
        private const string theContentPlanName = "this one";
        private const string theContents = "some contents";

        protected override void beforeEach()
        {
            theContinuation = MockRepository.GenerateMock<ProcessContentAction>();
            theFiles = new AssetFile[]{new AssetFile("something.js"), new AssetFile("other.css")};

            var theSource = MockFor<IContentSource>();
            MockFor<IContentPlanCache>().Stub(x => x.SourceFor(theContentPlanName)).Return(theSource);

            theSource.Stub(x => x.GetContent(MockFor<IContentPipeline>())).Return(theContents);
            theSource.Stub(x => x.Files).Return(theFiles);

            ClassUnderTest.Execute(theContentPlanName, theContinuation);
        }

        [Test]
        public void should_call_back_through_the_continuation()
        {
            theContinuation.AssertWasCalled(x => x.Invoke(theContents, theFiles));
        }
    }
}