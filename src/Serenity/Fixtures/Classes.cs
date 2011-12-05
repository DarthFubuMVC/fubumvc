
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using FubuCore;
using StoryTeller.Engine;

namespace Serenity.Fixtures
{


    public interface IElementHandler
    {
        bool Matches(IWebElement element);
        void EnterData(IWebElement element, string data);
        string GetData(IWebElement element);
    }

    public class DefaultElementHandler : IElementHandler
    {
        public bool Matches(IWebElement element)
        {
            return true;
        }

        public void EnterData(IWebElement element, string data)
        {
            throw new NotImplementedException();
            //element.As<OpenQA.Selenium.>()
        }

        public string GetData(IWebElement element)
        {
            throw new NotImplementedException();
        }
    }

    public class EnterValueGrammar : SimpleElementGesture
    {
        public EnterValueGrammar(GestureConfig def) : base(def)
        {
        }

        protected override void execute(IWebElement element, IDictionary<string, object> cellValues)
        {
            throw new NotImplementedException();
        }

        public override IList<Cell> GetCells()
        {
            return new List<Cell>();
        }
    }
}
