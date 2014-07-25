using System.Linq;
using FubuCore;
using StructureMap.Query;

namespace FubuMVC.StructureMap.Diagnostics
{
    public class PluginTypeDTO
    {
        public PluginTypeDTO()
        {
        }

        public PluginTypeDTO(IPluginTypeConfiguration configuration)
        {
            profile = configuration.ProfileName;
            pluginType = configuration.PluginType.GetFullName();
            lifecycle = configuration.Lifecycle.GetType().Name.Replace("Lifecycle", "");

            if (configuration.Default != null)
            {
                defaultInstance = new InstanceDTO(configuration.Default);

                others = configuration.Instances.Where(x => x.Name != configuration.Default.Name)
                    .Select(x => new InstanceDTO(x))
                    .ToArray();
            }
            else
            {
                others = configuration.Instances.Select(x => new InstanceDTO(x))
                    .ToArray();
            }

            if (configuration.MissingNamedInstance != null)
            {
                missingName = new InstanceDTO(configuration.MissingNamedInstance);
            }

            if (configuration.Fallback != null)
            {
                fallback = new InstanceDTO(configuration.Fallback);
            }
        }

        public string profile { get; set; }
        public string pluginType { get; set; }
        public string lifecycle { get; set; }

        public InstanceDTO[] others { get; set; }

        public InstanceDTO defaultInstance { get; set; }
        public InstanceDTO missingName { get; set; }
        public InstanceDTO fallback { get; set; }
    }
}