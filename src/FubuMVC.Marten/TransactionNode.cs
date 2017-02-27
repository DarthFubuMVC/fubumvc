using System;
using System.Data;
using FubuMVC.Core.Registration.Nodes;
using StructureMap.Pipeline;

namespace FubuMVC.Marten
{
    public class TransactionNode : Wrapper
    {
        public TransactionNode() : base(typeof(TransactionalBehavior))
        {
        }

        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;
    }
}