using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class OutputNode : BehaviorNode
    {
        private readonly WriterChain _chain = new WriterChain();
        private readonly Type _outputType;

        public OutputNode(Type outputType)
        {
            _outputType = outputType;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Output; }
        }

        public WriterChain Writers
        {
            get { return _chain; }
        }



        protected override ObjectDef buildObjectDef()
        {
            var def = new ObjectDef(typeof (OutputBehavior<>), _outputType);

            var mediaType = typeof (IMedia<>).MakeGenericType(_outputType);
            var enumerableType = typeof (IEnumerable<>).MakeGenericType(mediaType);
            var dependency = new ListDependency(enumerableType);
            dependency.AddRange(Writers.OfType<IContainerModel>().Select(x => x.ToObjectDef(DiagnosticLevel.None)));

            def.Dependency(dependency);

            return def;
        }

        public WriteWithFormatter AddFormatter<T>() where T : IFormatter
        {

            var formatter = new WriteWithFormatter(_outputType, typeof (T));
            var existing = Writers.FirstOrDefault(x => x.Equals(formatter));
            if (existing != null)
            {
                return existing as WriteWithFormatter;
            }
            

            Writers.AddToEnd(formatter);

            return formatter;
        }

        public Writer AddWriter<T>()
        {
            var writerType = typeof (IMediaWriter<>).MakeGenericType(_outputType);
            if (!typeof(T).CanBeCastTo(writerType))
            {
                throw new ArgumentOutOfRangeException("{0} can not be case to {1}", typeof(T).FullName, writerType.FullName);
            }

            var writer = new Writer(typeof (T));

            Writers.AddToEnd(writer);

            return writer;
        }

        public WriteHtml AddHtml()
        {
            var write = new WriteHtml(_outputType);
            Writers.AddToEnd(write);

            return write;
        }
    }
}