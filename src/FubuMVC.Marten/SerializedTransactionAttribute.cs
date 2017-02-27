using System.Data;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Marten
{
    public class SerializedTransactionAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCallBase call)
        {
            var node = call.ParentChain().OfType<TransactionNode>().FirstOrDefault();
            if (node == null)
            {
                node = new TransactionNode();
                call.ParentChain().Prepend(node);
            }

            node.IsolationLevel = IsolationLevel.Serializable;
        }
    }
}