using System;
using FubuMVC.Core.Bootstrapping;

namespace FubuMVC.Core
{
    public interface IContainerFacilityExpression
    {
        FubuApplication ContainerFacility(IContainerFacility facility);
        FubuApplication ContainerFacility(Func<IContainerFacility> facilitySource);
    }
}