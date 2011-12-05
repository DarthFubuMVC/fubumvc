using System.Collections.Generic;
using OpenQA.Selenium;
using StoryTeller.Engine;

namespace Serenity.Fixtures
{
    public class EnterValueGrammar : SimpleElementGesture
    {
        private readonly Cell _cell;

        public EnterValueGrammar(GestureConfig def) : base(def)
        {
            _cell = new Cell(def.CellName, def.CellType);
        }

        protected override void execute(IWebElement element, IDictionary<string, object> cellValues)
        {
            assertCondition(element.Enabled, DisabledElementMessage);
            assertCondition(element.Displayed, HiddenElementMessage);

            var data = cellValues[Config.CellName];
            ElementHandlers.FindHandler(element).EnterData(element, data);
        }

        public override IList<Cell> GetCells()
        {
            return new List<Cell>{
                _cell
            };
        }
    }
}