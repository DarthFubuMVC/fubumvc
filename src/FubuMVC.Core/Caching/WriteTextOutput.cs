using System;
using System.IO;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public class WriteTextOutput : IRecordedHttpOutput, IRecordedTextOutput, DescribesItself
    {
        private readonly string _text;

        public WriteTextOutput(string text)
        {
            _text = text;
        }

        public void Replay(IHttpResponse response)
        {
            response.Write(_text);
        }

        public void WriteText(StringWriter writer)
        {
            writer.Write(_text);
        }

        public bool Equals(WriteTextOutput other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._text, _text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (WriteTextOutput)) return false;
            return Equals((WriteTextOutput) obj);
        }

        public override int GetHashCode()
        {
            return (_text != null ? _text.GetHashCode() : 0);
        }

        public void Describe(Description description)
        {
            description.Title = "Write Text";
            description.LongDescription = _text;
        }

        public override string ToString()
        {
            return string.Format("Text: {0}", _text);
        }
    }
}