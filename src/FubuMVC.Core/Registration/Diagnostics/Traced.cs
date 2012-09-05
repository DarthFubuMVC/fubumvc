using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class Traced : NodeEvent, DescribesItself
    {
        private readonly string _text;

        public Traced(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = _text;
        }
    }
}