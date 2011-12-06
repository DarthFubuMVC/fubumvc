using System;
using System.Collections;
using System.Linq.Expressions;
using FubuCore;
using FubuLocalization;
using OpenQA.Selenium;
using Serenity.Fixtures.Grammars;
using StoryTeller;
using StoryTeller.Assertions;
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

        public IGrammar Click(By selector = null, string id = null, string css = null, string name = null, string label = null, string template = null)
        {
            var by = selector ?? id.ById() ?? css.ByCss() ?? name.ByName();

            if (by == null) throw new InvalidOperationException("Must specify either the selector, css, or name property");

            label = label ?? by.ToString().Replace("By.", "");

            var config = new GestureConfig
            {
                Template = template ?? "Click " + label,
                Description = "Click " + label,
                Finder = () => SearchContext.FindElement(by),
                FinderDescription = by.ToString()
            };

            return new ClickGrammar(config);
        }

        public void PushElementContext(ISearchContext context)
        {
            _searchContexts.Push(context);
        }

        public void PushElementContext(By selector)
        {
            var element = SearchContext.FindElement(selector);
            StoryTellerAssert.Fail(element == null, () => "Unable to find element with " + selector);

            PushElementContext(element);
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
        private GestureConfig getGesture(Expression<Func<T, object>> expression, string label = null, string key = null)
        {
            // TODO -- later on, use the naming convention from fubu instead of pretending
            // that this rule is always true
            var config = GestureForProperty(expression);
            if (key.IsNotEmpty())
            {
                config.CellName = key;
            }

            config.Label = label ?? LocalizationManager.GetHeader(expression);

            return config;
        }


        public IGrammar EnterScreenValue(Expression<Func<T, object>> expression, string label = null, string key = null)
        {
            var config = getGesture(expression, label, key);

            config.Template = "Enter {" + config.CellName + "} for " + config.Label;
            config.Description = "Enter data for property " + expression.ToAccessor().Name;

            return new EnterValueGrammar(config);
        }

        public IGrammar CheckScreenValue(Expression<Func<T, object>> expression, string label = null, string key = null)
        {
            var config = getGesture(expression, label, key);

            config.Template = "The text of " + config.Label + " should be {" + config.CellName + "}";
            config.Description = "Check data for property " + expression.ToAccessor().Name;
            
            return new CheckValueGrammar(config);
        }



        public GestureConfig GestureForProperty(Expression<Func<T, object>> expression)
        {
            return GestureConfig.ByProperty(() => SearchContext, expression);
        }

    }
}