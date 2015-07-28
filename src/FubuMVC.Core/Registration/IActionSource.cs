using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Implementations of this contract inspect a given <see cref="TypePool"/>
    /// and provide any number of <see cref="ActionCall"/> instances.
    /// </summary>
    public interface IActionSource
    {
        Task<ActionCall[]> FindActions(Assembly applicationAssembly);
    }

}