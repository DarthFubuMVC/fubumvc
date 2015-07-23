using System;
using System.Collections.Generic;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using OpenQA.Selenium;
using Rhino.Mocks;
using Serenity.Fixtures;
using Serenity.Fixtures.Grammars;
using StoryTeller;
using StoryTeller.Conversion;
using StoryTeller.Results;

namespace Serenity.Testing.Fixtures.Grammars
{
    public class SimpleElementGestureTester
    {
        [TestFixture]
        public class ErrorScenarios : InteractionContext<ErrorGeneratingGesture>
        {
            [Test]
            public void AddsGestureConfigDescriptionOnError()
            {
                const string name = "Test-Name";

                var gesture = GestureConfig.ByName(name);
                gesture.Finder = driver => MockFor<IWebElement>();

                Services.Inject(gesture);

                var specContext = MockFor<ISpecContext>();
                var applicationUnderTest = MockFor<IApplicationUnderTest>();
                var fixture = MockFor<ScreenFixture>();

                fixture.Context = specContext;
                specContext.Stub(x => x.Service<IApplicationUnderTest>()).Return(applicationUnderTest);
                applicationUnderTest.Stub(x => x.Driver).Return(MockFor<IWebDriver>());

                var stepValues = new StepValues("id");

                fixture.SetUp();
                Services.Inject(fixture);

                var exception = Exception<Exception>.ShouldBeThrownBy(() => {
                    ClassUnderTest.Execute(stepValues, null);
                });

                exception.ToString().ShouldContain(name);
            }
        }

        public class ErrorGeneratingGesture : SimpleElementGesture
        {
            public ErrorGeneratingGesture(ScreenFixture fixture, GestureConfig config) : base(fixture, config)
            {
            }

            protected override IEnumerable<CellResult> execute(IWebElement element, StepValues values)
            {
                throw new Exception("Test exception handling");
            }
        }
    }
}
