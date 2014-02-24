using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Conneg
{
    [MarkedForTermination]
    public class WriterChain : Chain<WriterNode, WriterChain>
    {
    }
}