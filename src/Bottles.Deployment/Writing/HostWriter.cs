using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles.Deployment.Writing
{
    public class HostWriter
    {
        private readonly ITypeDescriptorCache _types;
        private readonly TextWriter _writer = new StringWriter();
        private readonly IFileSystem _fileSystem = new FileSystem();

        public HostWriter(ITypeDescriptorCache types)
        {
            _types = types;
        }

        public void WriteTo(HostDefinition host, string recipeDirectory)
        {
            var fileName = host.FileName;
            fileName = FileSystem.Combine(recipeDirectory, fileName);

            _fileSystem.CreateDirectory(recipeDirectory);

            host.References.Each(WriteReference);
            host.Values.Each(WritePropertyValue);
            host.Directives.Each(WriteDirective);

            _fileSystem.WriteStringToFile(fileName, ToText());
        }

        public void WriteReference(BottleReference reference)
        {
            var text = ProfileFiles.BottlePrefix + reference.Name;
            if (reference.Relationship.IsNotEmpty())
            {
                text += " " + reference.Relationship;
            }

            _writer.WriteLine(text);
        }

        public void WriteDirective(IDirective directive)
        {
            var directiveWriter = new DirectiveWriter(_writer, _types);
            directiveWriter.Write(directive);
        }

        public string ToText()
        {
            return _writer.ToString();
        }

        public IEnumerable<string> AllLines()
        {
            var lines = new List<string>();

            using (var reader = new StringReader(ToText()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        public void WritePropertyValue(PropertyValue value)
        {
            _writer.WriteLine(value.ToString());
        }
    }
}