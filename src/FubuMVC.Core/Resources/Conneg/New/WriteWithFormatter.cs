using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Formatters;

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

        public Type FormatterType
        {
            get { return _formatterType; }
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

        public bool Equals(WriteWithFormatter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._resourceType, _resourceType) && Equals(other._formatterType, _formatterType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (WriteWithFormatter)) return false;
            return Equals((WriteWithFormatter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_resourceType != null ? _resourceType.GetHashCode() : 0)*397) ^ (_formatterType != null ? _formatterType.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("ResourceType: {0}, FormatterType: {1}", _resourceType, _formatterType);
        }
    }
}