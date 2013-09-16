using Bottles.Services;

namespace FubuMVC.Core
{
    /// <summary>
    /// Used to share the construction of a FubuMVC application between different hosts.
    /// Can be used in ASP.Net as FubuApplication.Bootstrap<T>() where T : IApplicationSource.
    /// Putting your application bootstrapping behind an IApplicationSource makes it much 
    /// easier to run your application in embedded hosts like OWIN
    /// </summary>
    public interface IApplicationSource : IApplicationSource<FubuApplication, FubuRuntime>
    {
    }
}