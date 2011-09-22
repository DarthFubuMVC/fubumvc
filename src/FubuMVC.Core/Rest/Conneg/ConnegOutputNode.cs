using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Formatters;
using System.Linq;

namespace FubuMVC.Core.Rest.Conneg
{

    public class ConnegOutputNode : ConnegNode
    {
        private readonly IList<IMediaWriterNode> _writers = new List<IMediaWriterNode>();

        public ConnegOutputNode(Type inputType) : base(inputType)
        {
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Output; }
        }

        public void AddWriter(IMediaWriterNode node)
        {
            if (node.InputType != InputType)
            {
                throw new ArgumentOutOfRangeException("node", node, "InputType's do not match");
            }

            _writers.Add(node);
        }

        public IEnumerable<IMediaWriterNode> Writers
        {
            get
            {
                return _writers;
            }
        }

        protected override Type formatterActionType()
        {
            return typeof (FormatterMediaWriter<>);
        }

        protected override IEnumerable<ObjectDef> createBuilderDependencies()
        {
            return _writers.Select(x => x.ToObjectDef())
                .Union(createFormatterObjectDef());
        }

        protected override Type getReaderWriterType()
        {
            return typeof (IMediaWriter<>);
        }

        protected override Type behaviorType()
        {
            return typeof (ConnegOutputBehavior<>);
        }

        public void JsonOnly()
        {
            UseFormatter<JsonFormatter>();
        }
    }

    public interface IMediaWriterNode : IContainerModel
    {
        Type InputType { get; }
    }
}