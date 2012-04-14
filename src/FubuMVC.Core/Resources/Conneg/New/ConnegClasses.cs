using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

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
            throw new NotImplementedException();
            var def = new ObjectDef(typeof (OutputBehavior<>), _outputType);

            var mediaType = typeof (IMedia<>).MakeGenericType(_outputType);
            var enumerableType = typeof (IEnumerable<>).MakeGenericType(mediaType);
            var dependency = new ListDependency(enumerableType);
            dependency.AddRange(Writers.OfType<IContainerModel>().Select(x => x.ToObjectDef(DiagnosticLevel.None)));

            def.Dependency(dependency);

            return def;
        }
    }

    public interface IReader<T>
    {
        IEnumerable<string> Mimetypes { get; }
        T Read(string mimeType);
    }

    public abstract class InputNode : BehaviorNode
    {
        private readonly ReaderChain _readers = new ReaderChain();

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
        }
    }

    public class ReaderChain : Chain<ReaderNode, ReaderChain>
    {
    }

    public abstract class ReaderNode : Node<ReaderNode, ReaderChain>, IContainerModel
    {
        public abstract Type InputType { get; }
        public abstract IEnumerable<string> Mimetypes { get; }

        ObjectDef IContainerModel.ToObjectDef(DiagnosticLevel diagnosticLevel)
        {
            var objectDef = toReaderDef();

            // TODO -- validate that it's a Reader
            return objectDef;
        }

        protected abstract ObjectDef toReaderDef();
    }
}