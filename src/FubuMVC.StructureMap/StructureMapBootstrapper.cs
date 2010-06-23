using System;
using System.Collections.Generic;
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

        protected override IEnumerable<IFubuRegistryExtension> findExtensions()
        {
            // Just find all the IFubuRegistryExtension's from 
            // the StructureMap container
            return _smFacility.Container.GetAllInstances<IFubuRegistryExtension>();
        }

        public Func<IContainer, ServiceArguments, Guid, IActionBehavior> Builder { get { return _smFacility.Builder; } set { _smFacility.Builder = value; } }
    }
}