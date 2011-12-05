using System;
using System.Collections;
using FubuCore;
using OpenQA.Selenium;
using StoryTeller.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Fixtures
{
    public static class DateTimeExtensions
    {
        public static TimeSpan Minutes(this int number)
        {
            return new TimeSpan(0, 0, number, 0);
        }

        public static TimeSpan Hours(this int number)
        {
            return new TimeSpan(0, number, 0, 0);
        }

        public static TimeSpan Days(this int number)
        {
            return new TimeSpan(number, 0, 0, 0);
        }

        public static TimeSpan Seconds(this int number)
        {
            return new TimeSpan(0, 0, number);
        }
    }

    public class ScreenFixture : Fixture
    {
        private IApplicationUnderTest _application;
        private readonly Stack<ISearchContext> _searchContexts = new Stack<ISearchContext>();

        public override void SetUp(ITestContext context)
        {
            // TODO -- later, make this thing be able to swap up the application under test
            _application = context.Retrieve<IApplicationUnderTest>();
            
        }

        // TODO -- we'll need to push/pop this stuff shortly
        public ISearchContext SearchContext
        {
            get
            {
                if (!_searchContexts.Any())
                {
                    _searchContexts.Push(_application.Driver);
                }

                return _searchContexts.Peek();
            }
        }

        protected void pushSearchContext(ISearchContext context)
        {
            _searchContexts.Push(context);
        }

        protected void popSearchContext(ISearchContext context)
        {
            _searchContexts.Pop();
        }



        public IApplicationUnderTest Application
        {
            get { return _application; }
        }

        
    }

    public class ScreenFixture<T> : ScreenFixture{}
}