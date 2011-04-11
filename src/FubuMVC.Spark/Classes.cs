using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Parsing;
using FubuMVC.Spark.Scanning;
using Spark;
using Spark.FileSystem;

namespace FubuMVC.Spark
{

    // TODO: NOT THE FINAL CLASS, SUBJECT OF FURTHER REFACTORING AND REFINEMENT
    public class SparkViewOutput
    {
        private readonly SparkFile _file;
        private readonly IElementNodeExtractor _extractor;
        private readonly IFileSystem _fileSystem;

        public SparkViewOutput(SparkFile file,IElementNodeExtractor extractor,IFileSystem fileSystem)
        {
            _file = file;
            _extractor = extractor;
            _fileSystem = fileSystem;
        }

        public Stream Render()
        {
            return getStream(false);
        }

        public Stream RenderPartial()
        {
            return getStream(true);
        }


        private Stream getStream(bool isPartial)
        {
            var engine = new SparkViewEngine();
            var descriptor = new SparkViewDescriptor();
         
            var templates = new List<string>();
            var viewTemplate = _file.Path.Replace(_file.Root, "").TrimStart(Path.DirectorySeparatorChar);
            templates.Add(viewTemplate);
            if (!isPartial)
            {

                var master = _extractor.ExtractByName(_fileSystem.ReadStringFromFile(_file.Path), "use")
                    .Select(x => x.AttributeByName("master")).FirstOrDefault();
                if (master != null)
                {
                    //templates.Add(Path.Combine("Scanning", "Templates", "Shared", master));
                }
            }

            engine.ViewFolder = new FileSystemViewFolder(_file.Root);
            foreach (var template in templates)
            {
                descriptor.AddTemplate(template);
            }
            var entry = engine.CreateEntry(descriptor);
            var instance = entry.CreateInstance();
            var writer = new StringWriter();
            instance.RenderView(writer);

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(writer.ToString());
            return ms;
        }

    }

}