using System.Collections.Generic;

namespace FubuMVC.Core.Environment
{
    public interface IEnvironmentRequirements
    {
        IEnumerable<IEnvironmentRequirement> Requirements();
    }
}