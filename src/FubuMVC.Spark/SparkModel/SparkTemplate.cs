using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using Spark;

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
        private readonly SparkEngineSettings _settings;

        public SparkTemplate(IFubuApplicationFiles files, IFubuFile file, ISparkViewEngine engine, SparkEngineSettings settings)
            : base(file, files)
        {
            _settings = settings;
            _full = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(true));
            _partial = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(false));

            _viewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_full.Value));
            _partialViewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_partial.Value));
        }

        public override IRenderableView GetView()
        {
            return _viewEntry.Value.CreateInstance().As<IRenderableView>();
        }

        public override IRenderableView GetPartialView()
        {
            return _partialViewEntry.Value.CreateInstance().As<IRenderableView>();
        }


        protected override Parsing createParsing(IFubuApplicationFiles files)
        {
            if (RelativePath().IsEmpty())
            {
                return new Parsing
                {
                    Master = null,
                    Namespaces = new String[0],
                    ViewModelType = null
                };
            }

            try
            {
                var chunk = Retry.Times(_settings.RetryViewLoadingCount, () => Loader.Load(this, files)).ToList();

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
            _viewEntry.Precompile();
            _partialViewEntry.Precompile();
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

        internal static class Retry
        {
            public static T Times<T>(int times, Func<T> action)
            {
                if (times == 0)
                    return action();

                var count = 0;
                while (count < times)
                {
                    try
                    {
                        return action();
                    }
                    catch (Exception)
                    {
                        if (count >= (times - 1))
                        {
                            throw;
                        }

                        count++;
                        Thread.Sleep(100);
                    }
                }

                throw new InvalidOperationException("Could not execute action");
            }
        }
    }

}
