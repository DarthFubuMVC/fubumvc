using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTemplate : ITemplateFile {}

    public class SparkTemplate : Template, ISparkTemplate
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
            var chunk = Loader.Load(this).ToList();

            return new Parsing
            {
                Master = chunk.Master(),
                ViewModelType = chunk.ViewModel(),
                Namespaces = chunk.Namespaces()
            };
        }

        public void Precompile()
        {
            _viewEntry.Precompile();
            _partialViewEntry.Precompile();
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