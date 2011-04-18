using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Parsing;
using Spark;
using Spark.FileSystem;

namespace FubuMVC.Spark.Tests.Experiments
{
    public class SparkViewOutput
    {
        private readonly SparkItem _item;
        private readonly IElementNodeExtractor _extractor;
        private readonly IFileSystem _fileSystem;

        public SparkViewOutput(SparkItem item,IElementNodeExtractor extractor,IFileSystem fileSystem)
        {
            _item = item;
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
            var viewTemplate = _item.Path.Replace(_item.Root, "").TrimStart(Path.DirectorySeparatorChar);
            templates.Add(viewTemplate);
            if (!isPartial)
            {

                var master = _extractor.ExtractByName(_fileSystem.ReadStringFromFile(_item.Path), "use")
                    .Select(x => x.AttributeByName("master")).FirstOrDefault();
                if (master != null)
                {
                    //templates.Add(Path.Combine("Scanning", "Templates", "Shared", master));
                }
            }

            engine.ViewFolder = new FileSystemViewFolder(_item.Root);
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