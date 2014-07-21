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
            description.ShortDescription = null;
            description.Properties["Mimetypes"] = Mimetypes.Join(", ");
            description.Children["Writer"] = Description.For(_writer);
            description.Children["Condition"] = Description.For(_condition);
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _writer.Mimetypes; }
        }

        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            _writer.Write(mimeType, context, resource);
        }

        public bool MatchesRequest(IFubuRequestContext context)
        {
            return _condition.ShouldExecute(context);
        }

        public IConditional Condition
        {
            get { return _condition; }
        }

        object IMedia.Writer
        {
            get { return Writer; }
        }


        private string debuggerDisplay()
        {
            return Writer.Mimetypes.Join(",");
        }
    }
}