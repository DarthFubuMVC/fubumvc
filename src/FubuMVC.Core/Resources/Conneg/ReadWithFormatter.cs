using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ReadWithFormatter : ReaderNode
    {
        private readonly Type _formatterType;
        private readonly Type _inputType;

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

        public override IEnumerable<string> Mimetypes
        {
            get { return MimeTypeAttribute.ReadFrom(_formatterType); }
        }

        public Type FormatterType
        {
            get { return _formatterType; }
        }

        public override Type InputType
        {
            get { return _inputType; }
        }

        protected override ObjectDef toReaderDef()
        {
            return new ObjectDef(typeof (FormatterReader<,>), _inputType, _formatterType);
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
                return ((_inputType != null ? _inputType.GetHashCode() : 0)*397) ^
                       (_formatterType != null ? _formatterType.GetHashCode() : 0);
            }
        }
    }
}