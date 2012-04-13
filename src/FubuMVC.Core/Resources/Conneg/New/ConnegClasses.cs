using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media;

namespace FubuMVC.Core.Resources.Conneg.New
{
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

    public abstract class WriterNode : Node<WriterNode, WriterChain>, IContainerModel
    {
        public abstract Type InputType { get; }

        ObjectDef IContainerModel.ToObjectDef(DiagnosticLevel diagnosticLevel)
        {
            return toObjectDef(diagnosticLevel);
        }

        protected abstract ObjectDef toObjectDef(DiagnosticLevel diagnosticLevel);
    }
}