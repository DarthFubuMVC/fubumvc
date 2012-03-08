namespace FubuMVC.Core.Registration.Diagnostics
{
    public abstract class NodeEvent
    {
        private readonly object _subject;

        public NodeEvent(object subject)
        {
            _subject = subject;
        }

        public object Subject
        {
            get { return _subject; }
        }

        public ConfigSource Source { get; set; }
    }
}