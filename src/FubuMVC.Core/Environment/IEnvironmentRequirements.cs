using System.Collections.Generic;

namespace Bottles.Environment
{
    public interface IEnvironmentRequirements
    {
        IEnumerable<IEnvironmentRequirement> Requirements();
    }
}