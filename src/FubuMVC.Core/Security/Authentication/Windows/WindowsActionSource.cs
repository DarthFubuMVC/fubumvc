using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class WindowsActionSource : IActionSource
    {
        Task<ActionCall[]> IActionSource.FindActions(Assembly applicationAssembly)
        {
            return Task.FromResult(new[] {ActionCall.For<WindowsController>(x => x.Login(null))});
        }
    }
}