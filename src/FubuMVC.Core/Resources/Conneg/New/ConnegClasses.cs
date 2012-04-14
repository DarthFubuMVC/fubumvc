using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Resources.Conneg.New
{
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