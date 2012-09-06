using System.Collections.Generic;
using System.Diagnostics;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg
{
    [DebuggerDisplay("{debuggerDisplay()}")]
    public class Media<T> : IMedia<T>, DescribesItself
    {
        private readonly IConditional _condition;
        private readonly IMediaWriter<T> _writer;

        public Media(IMediaWriter<T> writer, IConditional condition)
        {
            _writer = writer;
            _condition = condition;
        }

        public IMediaWriter<T> Writer
        {
            get { return _writer; }
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = "Media Writer for " + typeof (T).Name;
            description.Children["Writer"] = Description.For(_writer);
            description.Children["Condition"] = Description.For(_condition);
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _writer.Mimetypes; }
        }

        public void Write(string mimeType, T resource)
        {
            _writer.Write(mimeType, resource);
        }

        public bool MatchesRequest()
        {
            return _condition.ShouldExecute();
        }

        public IConditional Condition
        {
            get { return _condition; }
        }

        private string debuggerDisplay()
        {
            return Writer.Mimetypes.Join(",");
        }
    }
}