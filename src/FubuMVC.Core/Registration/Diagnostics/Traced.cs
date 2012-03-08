namespace FubuMVC.Core.Registration.Diagnostics
{
    public class Traced : NodeEvent
    {
        private readonly string _text;

        public Traced(string text, object subject) : base(subject)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }
    }
}