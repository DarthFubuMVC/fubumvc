using System;
using System.IO;
using System.Text;

namespace FubuMVC.Razor.Rendering
{
    public class TemplateHelper
    {
        private readonly Action<TextWriter> _write;

        public TemplateHelper(Action<TextWriter> write)
        {
            _write = write;
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                _write(writer);
                return writer.ToString();
            }
        }

        public void WriteTo(StringBuilder stringBuilder)
        {
            stringBuilder.Append(ToString());
        }
    }
}