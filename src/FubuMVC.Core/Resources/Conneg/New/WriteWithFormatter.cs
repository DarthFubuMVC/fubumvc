using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class WriteWithFormatter : WriterNode
    {
        private readonly Type _resourceType;
        private readonly Type _formatterType;

        public WriteWithFormatter(Type resourceType, Type formatterType)
        {
            if (resourceType == null)
            {
                throw new ArgumentNullException();
            }

            if (!formatterType.CanBeCastTo<IFormatter>())
            {
                throw new ArgumentOutOfRangeException("formatterType", "formatterType must be assignable to IFormatter");
            }

            _resourceType = resourceType;
            _formatterType = formatterType;
        }

        public override Type ResourceType
        {
            get { return _resourceType; }
        }

        protected override ObjectDef toWriterDef()
        {
            return new ObjectDef(typeof(FormatterWriter<,>), _resourceType, _formatterType);
        }

        public override IEnumerable<string> Mimetypes
        {
            get { return MimeTypeAttribute.ReadFrom(_formatterType); }
        }
    }
}