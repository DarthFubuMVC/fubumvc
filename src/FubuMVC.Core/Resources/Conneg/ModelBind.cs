using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    [Title("Model Binding")]
    [Description("Reads the input model by applying model binding against the request data")]
    public class ModelBind : ReaderNode
    {
        private readonly Type _inputType;

        public ModelBind(Type inputType)
        {
            if (inputType == null)
            {
                throw new ArgumentNullException();
            }

            _inputType = inputType;
        }

        public override Type InputType
        {
            get { return _inputType; }
        }

        protected override ObjectDef toReaderDef()
        {
            return new ObjectDef(typeof(ModelBindingReader<>), _inputType);
        }

        public override IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.HttpFormMimetype;
                yield return MimeType.MultipartMimetype;
            }
        }
    }
}