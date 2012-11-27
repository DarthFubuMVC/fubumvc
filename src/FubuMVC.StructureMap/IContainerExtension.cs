using StructureMap;

namespace FubuMVC.StructureMap
{
    // TODO -- this needs to be in SM3 itself
    public interface IContainerExtension
    {
        void Extend(IContainer container);
    }
}