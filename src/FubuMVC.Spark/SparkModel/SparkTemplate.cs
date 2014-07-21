using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using Spark;
using Spark.Compiler;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTemplate : ITemplateFile {}

    public class SparkTemplate : Template, ISparkTemplate, DescribesItself
    {
        private static readonly ChunkLoader Loader = new ChunkLoader();
        private readonly Lazy<SparkViewDescriptor> _full;
        private readonly Lazy<SparkViewDescriptor> _partial;
        private readonly IList<ISparkTemplate> _bindings = new List<ISparkTemplate>();
        private readonly WatchedSparkEntry _viewEntry;
        private readonly WatchedSparkEntry _partialViewEntry;

        public SparkTemplate(IFubuFile file, ISparkViewEngine engine)
            : base(file)
        {
            _full = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(true));
            _partial = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(false));

            _viewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_full.Value));
            _partialViewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_partial.Value));
        }

        public SparkTemplate(string filepath, string rootpath, string origin) : this(new FubuFile(filepath, origin){ProvenancePath = rootpath}, new SparkViewEngine())
        {
        }

        public override IRenderableView GetView()
        {
            return _viewEntry.Value.CreateInstance().As<IRenderableView>();
        }

        public override IRenderableView GetPartialView()
        {
            return _partialViewEntry.Value.CreateInstance().As<IRenderableView>();
        }


        protected override Parsing createParsing()
        {
            try
            {
                IEnumerable<Chunk> chunk = null;

                try
                {
                    // A retry here.
                    chunk = Loader.Load(this) ?? Loader.Load(this);
                }
                catch (Exception)
                {
                    // Retry ONCE.
                    chunk = Loader.Load(this);
                }

                chunk = chunk.ToList();


                return new Parsing
                {
                    Master = chunk.Master(),
                    ViewModelType = chunk.ViewModel(),
                    Namespaces = chunk.Namespaces()
                };
            }
            catch (Exception e)
            {
                throw new Exception("Failed while trying to parse template file " + FilePath, e);
            }
        }

        public void Precompile()
        {
            try
            {
                _viewEntry.Precompile();
                _partialViewEntry.Precompile();
            }
            catch (Exception e)
            {
                PackageRegistry.Diagnostics.LogFor(this).MarkFailure(e);
            }
        }

        public override string ToString()
        {
            return "Spark Template File: " + FilePath;
        }

        public void Describe(Description description)
        {
            description.Title = RelativePath();
        }

        public void AddBinding(ISparkTemplate template)
        {
            _bindings.Add(template);
        }

        public IEnumerable<ISparkTemplate> Bindings
        {
            get { return _bindings; }
        }


        public ISparkViewEntry ViewEntry
        {
            get { return _viewEntry.Value; }
        }

        public ISparkViewEntry PartialViewEntry
        {
            get { return _partialViewEntry.Value; }
        }


        private SparkViewDescriptor createSparkDescriptor(bool useMaster)
        {
            var sparkDescriptor = new SparkViewDescriptor().AddTemplate(ViewPath);
            if (useMaster && Master != null)
            {
                appendMasterPage(sparkDescriptor, (ISparkTemplate)Master);
            }

            return sparkDescriptor;
        }

        private static void appendMasterPage(SparkViewDescriptor descriptor, ISparkTemplate template)
        {
            if (template == null)
            {
                return;
            }

            descriptor.AddTemplate(template.ViewPath);
            appendMasterPage(descriptor, template.Master as ISparkTemplate);
        }
    }

}