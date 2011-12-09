using System.Collections.Generic;
using FubuCore;
using OpenQA.Selenium;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace Serenity.Fixtures.Grammars
{
    public abstract class SimpleElementGesture : LineGrammar
    {
        private readonly GestureConfig _config;
        public static readonly string DisabledElementMessage = "Found the element, but it was disabled and could not be clicked";
        public static readonly string HiddenElementMessage = "Found the element, but it was not visible and should not be clicked";
        public static readonly string NonexistentElementMessage = "Could not find the element";


        public SimpleElementGesture(GestureConfig config) : base(config.Template)
        {
            _config = config;
        }

        public ISearchContext CurrentContext
        {
            get
            {
                return _config.FindContext();
            }
        }

        public override string Description
        {
            get { return _config.Description; }
        }

        public GestureConfig Config
        {
            get { return _config; }
        }

        public override void Execute(IStep containerStep, ITestContext context)
        {
            if (_config.BeforeClick != null) _config.BeforeClick();


            try
            {
                var element = _config.Finder();

                var values = new Dictionary<string, object>();
                GetCells().Each(cell => cell.ReadArgument(context, containerStep, o => values.Add(cell.Key, o)));

                execute(element, values, containerStep, context);

                _config.AfterClick();
            }
            catch (NoSuchElementException)
            {
                assertCondition(false, NonexistentElementMessage);
            }
        }

        protected abstract void execute(IWebElement element, IDictionary<string, object> cellValues, IStep step, ITestContext context);

        protected void assertCondition(bool condition, string message)
        {
            if (condition) return;

            if (_config.FinderDescription.IsNotEmpty())
            {
                message += "\nElement at --> " + _config.FinderDescription;
            }
            StoryTellerAssert.Fail(message);
        }

        public override IList<Cell> GetCells()
        {
            return new List<Cell>();
        }
    }
}