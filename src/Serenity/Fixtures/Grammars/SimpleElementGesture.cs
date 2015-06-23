using System;
using System.Collections.Generic;
using FubuCore;
using OpenQA.Selenium;
using StoryTeller;
using StoryTeller.Conversion;
using StoryTeller.Grammars.Lines;
using StoryTeller.Model;
using StoryTeller.Results;

namespace Serenity.Fixtures.Grammars
{
    public abstract class SimpleElementGesture : LineGrammar
    {
        private readonly ScreenFixture _fixture;
        private readonly GestureConfig _config;

        public static readonly string DisabledElementMessage =
            "Found the element, but it was disabled and could not be manipulated";

        public static readonly string HiddenElementMessage =
            "Found the element, but it was not visible and should not be manipulated";

        public static readonly string NonexistentElementMessage = "Could not find the element";
        private Cell _cell;


        public SimpleElementGesture(ScreenFixture fixture, GestureConfig config)
        {
            _fixture = fixture;
            _config = config;
        }

        protected ISearchContext SearchContext
        {
            get { return _fixture.SearchContext; }
        }

        public GestureConfig Config
        {
            get { return _config; }
        }

        protected override string format()
        {
            return _config.Template;
        }

        protected override IEnumerable<Cell> buildCells(CellHandling cellHandling, Fixture fixture)
        {
            _cell = new Cell(cellHandling, _config.CellName, _config.CellType) {output = isOutput};

            yield return _cell;
        }

        public Cell Cell
        {
            get { return _cell; }
        }

        protected virtual bool isOutput
        {
            get { return false; }
        }

        public override IEnumerable<CellResult> Execute(StepValues values, ISpecContext context)
        {
            var element = _config.Finder(SearchContext);
            StoryTellerAssert.Fail(element == null, "Could not find an element w/ description: " + _config.Description);

            try
            {
                return execute(element, values);
            }
            catch (Exception ex)
            {
                // TODO: Use exception metadata with and add a custom exception formatter to show exception metadata
                //       See: https://github.com/storyteller/Storyteller/issues/400
                // ex.Data.Add("Gesture Description", _config.Description);
                throw new Exception("Gesture failed. [Description: {0}]".ToFormat(_config.Description), ex);
            }
        }

        protected void assertCondition(bool condition, string message)
        {
            if (condition) return;

            if (_config.Description.IsNotEmpty())
            {
                message += "\nElement at --> " + _config.Description;
            }

            StoryTellerAssert.Fail(message);
        }

        protected abstract IEnumerable<CellResult> execute(IWebElement element, StepValues values);
    }
}
