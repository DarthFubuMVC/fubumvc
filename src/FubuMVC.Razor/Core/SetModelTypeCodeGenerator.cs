using System;
using System.Globalization;
using System.Web.Razor.Generator;

namespace FubuMVC.Razor.Core
{
    internal class SetModelTypeCodeGenerator : SetBaseTypeCodeGenerator
    {
        private readonly string _genericTypeFormat;

        public SetModelTypeCodeGenerator(string modelType, string genericTypeFormat)
            : base(modelType)
        {
            _genericTypeFormat = genericTypeFormat;
            ModelType = modelType;
        }

        public string ModelType { get; private set; }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                _genericTypeFormat,
                context.Host.DefaultBaseClass,
                baseType);
        }

        public override bool Equals(object obj)
        {
            SetModelTypeCodeGenerator other = obj as SetModelTypeCodeGenerator;
            return other != null &&
                base.Equals(obj) &&
                String.Equals(_genericTypeFormat, other._genericTypeFormat, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}", base.GetHashCode(), _genericTypeFormat).GetHashCode();
        }

        public override string ToString()
        {
            return "Model:" + BaseType;
        }
    }
}