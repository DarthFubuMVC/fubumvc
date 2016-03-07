using FubuCore.Binding;
using StructureMap;

namespace FubuMVC.Marten
{
    public static class ContainerExtensions
    {
        public static void Apply(this IContainer container, ServiceArguments arguments)
        {
            arguments.EachService(container.Inject);
        }

    }
}