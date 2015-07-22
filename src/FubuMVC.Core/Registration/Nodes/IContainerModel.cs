using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration.Nodes
{
    public interface IContainerModel
    {
        Instance ToInstance();
    }
}