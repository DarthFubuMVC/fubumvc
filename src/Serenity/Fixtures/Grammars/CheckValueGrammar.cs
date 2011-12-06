using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Serenity.Fixtures.Handlers;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace Serenity.Fixtures.Grammars
{
    public class CheckValueGrammar : SimpleElementGesture
    {
        private readonly Cell _cell;

        public CheckValueGrammar(GestureConfig config) : base(config)
        {
            _cell = new Cell(config.CellName, config.CellType){
                IsResult = true
            };
        }

        protected override void execute(IWebElement element, IDictionary<string, object> cellValues, IStep step, ITestContext context)
        {
            // TODO -- StoryTeller needs to pull this all inside the Cell
            if (!cellValues.ContainsKey(_cell.Key))
            {
                // already caught as a syntax error
                return;
            }

            var handler = ElementHandlers.FindHandler(element);
            var expectedValue = cellValues[_cell.Key];

            var matchingHandler = handler as IMatchingHandler ?? new BasicMatchingHandler(handler, context);

            if (matchingHandler.MatchesData(element, expectedValue))
            {
                context.IncrementRights();
            }
            else
            {
                context.ResultsFor(step).MarkFailure(_cell.Key);
                context.IncrementWrongs();
            }

            context.ResultsFor(step).SetActual(_cell.Key, handler.GetData(element));
        }


        public override IList<Cell> GetCells()
        {
            return new List<Cell>{_cell};
        }
    }
}