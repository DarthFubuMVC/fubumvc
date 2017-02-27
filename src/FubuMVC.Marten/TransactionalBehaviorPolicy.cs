using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Marten
{
    public class TransactionalBehaviorPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Routes.Commands.Where(x => !x.OfType<TransactionNode>().Any()).Each(x => x.InsertFirst(new TransactionNode()));
        }
    }
}