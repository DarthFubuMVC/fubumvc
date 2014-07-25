using System;
using FubuCore;
using StructureMap.Query;

namespace FubuMVC.StructureMap.Diagnostics
{
    public class InstanceDTO
    {
        public InstanceDTO()
        {
        }

        public InstanceDTO(InstanceRef model)
        {
            name = model.Name;
            lifecycle = model.Lifecycle.GetType().Name.Replace("Lifecycle", "");
            returnedType = model.ReturnedType.GetFullName();
            description = model.Description;
            hasBeenCreated = model.ObjectHasBeenCreated();
            pluginType = model.PluginType.GetFullName();

            Guid ignored;
            if (Guid.TryParse(name, out ignored))
            {
                name = "(guid)";
            }

            key = "{0}/{1}".ToFormat(model.PluginType.GetFullName(), model.Name);
        }

        public string name { get; set; }
        public string lifecycle { get; set; }
        public string returnedType { get; set; }
        public string description { get; set; }
        public bool hasBeenCreated { get; set; }
        public string pluginType { get; set; }
        public string key { get; set; }
    }
}