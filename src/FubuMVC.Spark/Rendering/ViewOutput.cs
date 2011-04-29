using System.IO;
using System.Text;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Spark.Rendering
{
    public class ViewOutput : TextWriter
    {
        private readonly IOutputWriter _outputWriter;
        public ViewOutput(IOutputWriter outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
		
        public override void Write(string value)
        {
            _outputWriter.WriteHtml(value);
        }
    }
}