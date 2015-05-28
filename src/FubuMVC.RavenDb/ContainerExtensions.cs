using FubuCore.Binding;
using FubuPersistence.RavenDb;
using Raven.Client;
using StructureMap;

namespace FubuPersistence
{
    public static class ContainerExtensions
    {
        public static void Apply(this IContainer container, ServiceArguments arguments)
        {
            arguments.EachService(container.Inject);
        }

    }
}