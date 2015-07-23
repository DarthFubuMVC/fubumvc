using System;
using System.Collections.Generic;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using OpenQA.Selenium;
using Rhino.Mocks;
using Serenity.Fixtures.Handlers;

namespace Serenity.Testing.Fixtures.Handlers
{
    public class ElementHandlerWrapperTester : InteractionContext<ElementHandlerWrapperTester.TestElementHandlerWrapper>
    {
        private const string Result = "Some returned text";

        private IWebElement _element;
        private ISearchContext _context;
        private object _data;

        protected override void beforeEach()
        {
            _element = MockFor<IWebElement>();
            _context = MockFor<ISearchContext>();
            _data = new object();

            ClassUnderTest.Element = _element;
        }

        [Test]
        public void DoesNotCallNestedMatchesIfWrapperDoesNotMatch()
        {
            ElementHandlerWrapper.AllHandlers = () =>
            {
                throw new Exception("Should not attempt to get all handlers");
            };

            ClassUnderTest.MatchesResult = false;

            ClassUnderTest.Matches(_element).ShouldBeFalse();
        }

        [Test]
        public void DoesNotMatchIfThereAreNoMatchingInnerHandlers()
        {
            var handlers = new List<IElementHandler>
            {
                ClassUnderTest,
                MockedHandler(false),
                MockedHandler(false)
            };

            ElementHandlerWrapper.AllHandlers = () => handlers;

            ClassUnderTest.Matches(_element).ShouldBeFalse();
            handlers[1].AssertWasCalled(x => x.Matches(_element));
            handlers[2].AssertWasCalled(x => x.Matches(_element));
        }

        [Test]
        public void MatchesOnlyConsidersHandlersAfterItself()
        {
            var handlers = new List<IElementHandler>
            {
                MockedHandler(true),
                ClassUnderTest,
                MockedHandler(false)
            };

            ElementHandlerWrapper.AllHandlers = () => handlers;

            ClassUnderTest.Matches(_element).ShouldBeFalse();
            handlers[0].AssertWasNotCalled(x => x.Matches(_element));
            handlers[2].AssertWasCalled(x => x.Matches(_element));
        }

        [Test]
        public void Matches()
        {
            var handlers = new List<IElementHandler>
            {
                ClassUnderTest,
                MockedHandler(false),
                MockedHandler(true),
                MockedHandler(false)
            };

            ElementHandlerWrapper.AllHandlers = () => handlers;

            ClassUnderTest.Matches(_element).ShouldBeTrue();
            handlers[1].AssertWasCalled(x => x.Matches(_element));
            handlers[2].AssertWasCalled(x => x.Matches(_element));
            handlers[3].AssertWasNotCalled(x => x.Matches(_element));
        }

        [Test]
        public void EnterDataEntersNestedData()
        {
            var handlers = new List<IElementHandler>
            {
                ClassUnderTest,
                MockedHandler(false),
                MockedHandler(true),
                MockedHandler(false)
            };

            ElementHandlerWrapper.AllHandlers = () => handlers;

            ClassUnderTest.EnterData(_context, _element, _data);

            handlers[1].AssertWasCalled(x => x.Matches(_element));
            handlers[2].AssertWasCalled(x => x.Matches(_element));
            handlers[3].AssertWasNotCalled(x => x.Matches(_element));

            handlers[1].AssertWasNotCalled(x => x.EnterData(_context, _element, _data));
            handlers[2].AssertWasCalled(x => x.EnterData(_context, _element, _data));
            handlers[3].AssertWasNotCalled(x => x.EnterData(_context, _element, _data));
        }

        [Test]
        public void GetDataGetsNestedData()
        {
            var handlers = new List<IElementHandler>
            {
                ClassUnderTest,
                MockedHandler(false),
                MockedHandler(true),
                MockedHandler(false)
            };

            ElementHandlerWrapper.AllHandlers = () => handlers;

            ClassUnderTest.GetData(_context, _element).ShouldBe(Result);

            handlers[1].AssertWasCalled(x => x.Matches(_element));
            handlers[2].AssertWasCalled(x => x.Matches(_element));
            handlers[3].AssertWasNotCalled(x => x.Matches(_element));

            handlers[1].AssertWasNotCalled(x => x.GetData(_context, _element));
            handlers[2].AssertWasCalled(x => x.GetData(_context, _element));
            handlers[3].AssertWasNotCalled(x => x.GetData(_context, _element));
        }

        private IElementHandler MockedHandler(bool matches)
        {
            var handler = MockedHandler();
            handler.Stub(x => x.Matches(_element)).Return(matches);

            if (matches)
            {
                handler.Stub(x => x.GetData(_context, _element)).Return(Result);
            }

            return handler;
        }

        private IElementHandler MockedHandler()
        {
            return Services.AddAdditionalMockFor<IElementHandler>();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            ElementHandlerWrapper.ResetAllHandlers();
        }

        public class TestElementHandlerWrapper : ElementHandlerWrapper
        {
            public bool MatchesResult { get; set; }

            public IWebElement Element { get; set; }

            public TestElementHandlerWrapper()
            {
                MatchesResult = true;
            }

            protected override bool WrapperMatches(IWebElement element)
            {
                ReferenceEquals(Element, element).ShouldBeTrue();
                return MatchesResult;
            }

            public override void EnterData(ISearchContext context, IWebElement element, object data)
            {
                EnterDataNested(context, element, data);
            }

            public override string GetData(ISearchContext context, IWebElement element)
            {
                return GetDataNested(context, element);
            }
        }
    }
}