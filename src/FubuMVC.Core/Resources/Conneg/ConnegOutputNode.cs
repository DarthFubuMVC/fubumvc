using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg
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

        public IEnumerable<IMediaWriterNode> Writers
        {
            get { return _writers; }
        }

        public void AddWriter(IMediaWriterNode node)
        {
            if (node.InputType != InputType)
            {
                throw new ArgumentOutOfRangeException("node", node, "InputType's do not match");
            }

            _writers.Add(node);
        }

        protected override Type formatterActionType()
        {
            return typeof (FormatterMediaWriter<>);
        }

        protected override IEnumerable<ObjectDef> createBuilderDependencies()
        {
            return _writers.Select(x => x.ToObjectDef(DiagnosticLevel.None))
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
}