using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Resources.Conneg.New
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
    }
}