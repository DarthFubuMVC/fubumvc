using System;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using StructureMap;

namespace FubuMVC.StructureMap
{
    public class StructureMapBootstrapper : FubuBootstrapper
    {
        private readonly StructureMapContainerFacility _smFacility;

        public StructureMapBootstrapper(IContainer container, FubuRegistry topRegistry)
            : this(new StructureMapContainerFacility(container), topRegistry)
        {
        }

        public StructureMapBootstrapper(StructureMapContainerFacility facility, FubuRegistry registry)
            : base(facility, registry)
        {
            _smFacility = facility;
        }

        public Func<IContainer, ServiceArguments, Guid, IActionBehavior> Builder { get { return _smFacility.Builder; } set { _smFacility.Builder = value; } }
    }
}