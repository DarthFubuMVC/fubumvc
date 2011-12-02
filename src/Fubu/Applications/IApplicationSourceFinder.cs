using FubuMVC.Core;

namespace Fubu.Applications
{
    public interface IApplicationSourceFinder
    {
        IApplicationSource FindSource(ApplicationSettings settings, ApplicationStartResponse theResponse);
    }
}