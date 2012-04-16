using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class ReadWithFormatter : ReaderNode
    {
        private readonly Type _inputType;
        private readonly Type _formatterType;

        public ReadWithFormatter(Type inputType, Type formatterType)
        {
            if (inputType == null)
            {
                throw new ArgumentNullException();
            }

            if (!formatterType.CanBeCastTo<IFormatter>())
            {
                throw new ArgumentOutOfRangeException("formatterType", "formatterType must be assignable to IFormatter");
            }

            _inputType = inputType;
            _formatterType = formatterType;
        }

        protected override ObjectDef toReaderDef()
        {
            return new ObjectDef(typeof(FormatterReader<,>), _inputType, _formatterType);
        }

        public override IEnumerable<string> Mimetypes
        {
            get
            {
                return MimeTypeAttribute.ReadFrom(_formatterType);
            }
        }

        public override Type InputType
        {
            get { return _inputType; }
        }

        public bool Equals(ReadWithFormatter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._inputType, _inputType) && Equals(other._formatterType, _formatterType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ReadWithFormatter)) return false;
            return Equals((ReadWithFormatter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_inputType != null ? _inputType.GetHashCode() : 0)*397) ^ (_formatterType != null ? _formatterType.GetHashCode() : 0);
            }
        }
    }
}