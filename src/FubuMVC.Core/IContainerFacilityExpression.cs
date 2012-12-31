using System;
using FubuMVC.Core.Bootstrapping;

namespace FubuMVC.Core
{
    public interface IContainerFacilityExpression
    {
        /// <summary>
        /// Attach an IContainerFacility to use in the application
        /// </summary>
        /// <param name="facility"></param>
        /// <returns></returns>
        FubuApplication ContainerFacility(IContainerFacility facility);

        /// <summary>
        /// Describe how the application should build its IContainerFacility
        /// </summary>
        /// <param name="facilitySource"></param>
        /// <returns></returns>
        FubuApplication ContainerFacility(Func<IContainerFacility> facilitySource);
    }
}