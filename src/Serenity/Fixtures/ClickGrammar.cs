using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using StoryTeller.Engine;
using FubuCore;

namespace Serenity.Fixtures
{
    public class ClickGrammar : LineGrammar
    {
        public static readonly string DisabledElementMessage = "Found the element, but it was disabled and could not be clicked";
        public static readonly string HiddenElementMessage = "Found the element, but it was not visible and should not be clicked";
        public static readonly string NonexistentElementMessage = "Could not find the element";

        private readonly Func<IWebElement> _finder;
        private Action _afterClick = () => { };
        private readonly string _description;
        private Action _beforeClick = () => { };

        public ClickGrammar(string description, Func<IWebElement> finder)
            : base(description)
        {
            _finder = finder;
            _description = description;
        }

        public string FinderDescription { get; set; }

        public Action BeforeClick
        {
            get { return _beforeClick; }
            set { _beforeClick = value; }
        }

        public Action AfterClick
        {
            get { return _afterClick; }
            set { _afterClick = value; }
        }

        public override void Execute(IStep containerStep, ITestContext context)
        {
            if (BeforeClick != null) BeforeClick();


            try
            {
                var element = _finder();

                assertCondition(element.Enabled, DisabledElementMessage);
                assertCondition(element.Displayed, HiddenElementMessage);

                element.Click();

                _afterClick();
            }
            catch (NoSuchElementException)
            {
                assertCondition(false, NonexistentElementMessage);
            }
        }

        private void assertCondition(bool condition, string message)
        {
            if (condition) return;

            if (FinderDescription.IsNotEmpty())
            {
                message += "\nElement at --> " + FinderDescription;
            }
            StoryTellerAssert.Fail(message);
        }

        public override IList<Cell> GetCells()
        {
            return new List<Cell>();
        }

        public override string Description
        {
            get { return _description; }
        }
    }
}