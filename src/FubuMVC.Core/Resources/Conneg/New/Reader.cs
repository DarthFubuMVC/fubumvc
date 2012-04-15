using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class Reader : ReaderNode
    {
        private readonly Type _inputType;
        private readonly Type _readerType;

        public Reader(Type readerType, Type inputType = null)
        {
            if (readerType == null)
            {
                throw new ArgumentNullException("readerType");
            }

            if (readerType.IsOpenGeneric())
            {
                if (inputType == null)
                {
                    throw new ArgumentNullException("inputType", "inputType is required if the ReaderType is an open generic");
                }

                _inputType = inputType;
                _readerType = readerType.MakeGenericType(inputType);
            }
            else
            {
                var @interface = readerType.FindInterfaceThatCloses(typeof(IReader<>));
                if (@interface == null)
                {
                    throw new ArgumentOutOfRangeException("readerType", "ReaderType must be assignable to IReader<>");
                }

                _readerType = readerType;
                _inputType = @interface.GetGenericArguments().First();
            }


        }

        public Type ReaderType
        {
            get { return _readerType; }
        }

        public override Type InputType
        {
            get { return _inputType; }
        }

        protected override ObjectDef toReaderDef()
        {
            return new ObjectDef(ReaderType);
        }

        public override IEnumerable<string> Mimetypes
        {
            get { return MimeTypeAttribute.ReadFrom(ReaderType); }
        }
    }
}