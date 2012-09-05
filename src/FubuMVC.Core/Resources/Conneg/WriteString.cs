using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class WriteString : WriterNode
    {
        public override Type ResourceType
        {
            get { return typeof (string); }
        }

        protected override ObjectDef toWriterDef()
        {
            return ObjectDef.ForType<StringWriter>();
        }

        public override IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Text.Value; }
        }

        protected override void createDescription(Description description)
        {
            description.ShortDescription = "Writes out a string value to the Http response as text/plain";
        }
    }
}