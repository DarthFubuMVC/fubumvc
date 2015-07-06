using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public interface IClientMessageCache
    {
        BehaviorChain FindChain(string messageName);
        IEnumerable<ClientMessagePath> AllClientMessages();
    }
}