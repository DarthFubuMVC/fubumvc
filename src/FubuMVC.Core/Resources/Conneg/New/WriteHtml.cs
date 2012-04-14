using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class WriteHtml : WriterNode
    {
        private readonly Type _resourceType;

        public WriteHtml(Type resourceType)
        {
            _resourceType = resourceType;
        }

        public override Type ResourceType
        {
            get { return _resourceType; }
        }

        protected override ObjectDef toWriterDef()
        {
            return new ObjectDef(typeof(HtmlStringWriter<>), _resourceType);
        }

        public override IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.Html.Value;
            }
        }
    }
}