namespace FubuMVC.StructureMap.Diagnostics
{
    public class PluginTypeDTO
    {
        public string profile { get; set; }
        public string pluginType { get; set; }
        public string lifecycle { get; set; }

        public InstanceDTO defaultInstance { get; set; }
        public InstanceDTO missingName { get; set; }
        public InstanceDTO fallback { get; set; }

    }
}