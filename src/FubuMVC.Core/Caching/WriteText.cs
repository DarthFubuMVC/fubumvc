using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public class WriteText : IRecordedHttpOutput
    {
        private readonly string _text;

        public WriteText(string text)
        {
            _text = text;
        }

        public void Replay(IHttpWriter writer)
        {
            writer.Write(_text);
        }

        public bool Equals(WriteText other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._text, _text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (WriteText)) return false;
            return Equals((WriteText) obj);
        }

        public override int GetHashCode()
        {
            return (_text != null ? _text.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Text: {0}", _text);
        }
    }
}