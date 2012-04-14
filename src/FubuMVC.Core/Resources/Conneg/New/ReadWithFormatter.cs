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
    }
}