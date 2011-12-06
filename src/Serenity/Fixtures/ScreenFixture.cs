using System;
using System.Collections;
using System.Linq.Expressions;
using FubuCore;
using FubuLocalization;
using OpenQA.Selenium;
using Serenity.Fixtures.Grammars;
using StoryTeller;
using StoryTeller.Engine;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;

namespace Serenity.Fixtures
{
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

        

        public void PushElementContext(ISearchContext context)
        {
            _searchContexts.Push(context);
        }

        protected void PopElementContext(ISearchContext context)
        {
            _searchContexts.Pop();
        }



        public IApplicationUnderTest Application
        {
            get { return _application; }
        }

        
    }

    public class ScreenFixture<T> : ScreenFixture
    {
        public IGrammar EnterScreenValue(Expression<Func<T, object>> expression, string label = null, string key = null)
        {
            label = label ?? LocalizationManager.GetHeader(expression);

            // TODO -- later on, use the naming convention from fubu instead of pretending
            // that this rule is always true
            var config = GestureForProperty(expression);
            if (key.IsNotEmpty())
            {
                config.CellName = key;
            }

            config.Template = "Enter {" + config.CellName + "} for " + label;
            config.Description = "Enter data for property " + expression.ToAccessor().Name;

            return new EnterValueGrammar(config);
        }

        public IGrammar CheckScreenValue(Expression<Func<T, object>> expression, string label = null, string key = null)
        {
            label = label ?? LocalizationManager.GetHeader(expression);

            // TODO -- later on, use the naming convention from fubu instead of pretending
            // that this rule is always true
            var config = GestureForProperty(expression);
            if (key.IsNotEmpty())
            {
                config.CellName = key;
            }

            config.Template = "The text of " + label + " should be {" + config.CellName + "}";
            config.Description = "Check data for property " + expression.ToAccessor().Name;

            return new CheckValueGrammar(config);
        }

        public GestureConfig GestureForProperty(Expression<Func<T, object>> expression)
        {
            return GestureConfig.ByProperty(() => SearchContext, expression);
        }
    }
}