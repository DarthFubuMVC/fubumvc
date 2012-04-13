using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media;
using System.Linq;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class OutputNode : BehaviorNode
    {
        private readonly Type _outputType;
        private readonly WriterChain _chain = new WriterChain();

        public OutputNode(Type outputType)
        {
            _outputType = outputType;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Output; }
        }

        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
            var def = new ObjectDef(typeof (OutputBehavior<>), _outputType);

            var mediaType = typeof (IMedia<>).MakeGenericType(_outputType);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(mediaType);
            var dependency = new ListDependency(enumerableType);
            dependency.AddRange(Writers.OfType<IContainerModel>().Select(x => x.ToObjectDef(DiagnosticLevel.None)));

            def.Dependency(dependency);

            return def;
        }

        public WriterChain Writers
        {
            get { return _chain; }
        }
    }


    public class ReaderChain : Chain<ReaderNode, ReaderChain>
    {
    }

    public abstract class ReaderNode : Node<ReaderNode, ReaderChain>, IContainerModel
    {
        public abstract Type OutputType { get; }

        ObjectDef IContainerModel.ToObjectDef(DiagnosticLevel diagnosticLevel)
        {
            return toObjectDef(diagnosticLevel);
        }

        protected abstract ObjectDef toObjectDef(DiagnosticLevel diagnosticLevel);
    }

    public class WriterChain : Chain<WriterNode, WriterChain>
    {
    }


    public class Writer : WriterNode
    {
        private readonly Type _resourceType;

        public Writer(Type resourceType, Type writerType)
        {
            _resourceType = resourceType;
        }

        public override Type ResourceType
        {
            get { return _resourceType; }
        }

        protected override ObjectDef toWriterDef()
        {
            throw new NotImplementedException();
        }
    }


}