using System;
using System.Xml;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Results
{
    [Obsolete]
    public class RenderRssOrAtomResult // : IInvocationResult
    {
        public static readonly string ATOM_CONTENT_TYPE = "application/atom+xml";
        public static readonly string RSS_CONTENT_TYPE = "application/rss+xml";
        private readonly string _contentType;
        private readonly Action<XmlWriter> _saveAs;

        public RenderRssOrAtomResult(Action<XmlWriter> saveAs, string contentType)
        {
            _saveAs = saveAs;
            _contentType = contentType;
        }


        //public void Execute(IServiceLocator locator)
        //{
        //    var writer = locator.GetInstance<IOutputWriter>();

        //    var stringBuilder = new StringBuilder();
        //    using (XmlWriter feedWriter = XmlWriter.Create(stringBuilder))
        //    {
        //        if (feedWriter != null)
        //        {
        //            _saveAs(feedWriter);
        //        }
        //    }
        //    string rss = stringBuilder.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
        //    // TODO: Make this nicer
        //    writeRssToOutput(writer, rss);
        //}


        protected virtual void writeRssToOutput(IOutputWriter writer, string rss)
        {
            writer.Write(_contentType, rss);
        }
    }
}