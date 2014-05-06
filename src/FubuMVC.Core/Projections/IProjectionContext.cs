using FubuCore.Formatting;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Projections
{
    public interface IProjectionContext<T>
    {
        /// <summary>
        /// Access to the underlying service locator of the running application
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService Service<TService>();
        IUrlRegistry Urls { get; }
        IDisplayFormatter Formatter { get; }

        IProjectionContext<TChild> ContextFor<TChild>(TChild child);

        IValues<T> Values { get; }
        T Subject { get; }
    }
}