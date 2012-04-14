using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media;
using System.Linq;
using FubuCore;

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
        private readonly Type _writerType;

        public Writer(Type writerType, Type resourceType = null)
        {
            if (writerType == null)
            {
                throw new ArgumentNullException("writerType");
            }

            if (writerType.IsOpenGeneric())
            {
                if (resourceType == null)
                {
                    throw new ArgumentNullException("resourceType", "resourceType is required if the writerType is an open generic");
                }

                _resourceType = resourceType;
                _writerType = writerType.MakeGenericType(resourceType);
            }
            else
            {
                var @interface = writerType.FindInterfaceThatCloses(typeof (IMediaWriter<>));
                if (@interface == null)
                {
                    throw new ArgumentOutOfRangeException("writerType", "writerType must be assignable to IMediaWriter<>");
                }

                _writerType = writerType;
                _resourceType = @interface.GetGenericArguments().First();
            }

            
        }

        public override Type ResourceType
        {
            get { return _resourceType; }
        }

        public Type WriterType
        {
            get { return _writerType; }
        }

        protected override ObjectDef toWriterDef()
        {
            return new ObjectDef(WriterType);
        }
    }


}