using FubuCore.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore
{
    public interface IDisplayFormatter
    {
        string GetDisplay(GetStringRequest request);
        string GetDisplay(Accessor accessor, object target);
    }

    public class DisplayFormatter : IDisplayFormatter
    {
        private readonly IServiceLocator _locator;
        private readonly Stringifier _stringifier;

        public DisplayFormatter(IServiceLocator locator, Stringifier stringifier)
        {
            _locator = locator;
            _stringifier = stringifier;
        }

        public string GetDisplay(GetStringRequest request)
        {
            request.Locator = _locator;
            return _stringifier.GetString(request);
        }

        public string GetDisplay(Accessor accessor, object target)
        {
            var request = new GetStringRequest(accessor, target, _locator);
            return _stringifier.GetString(request);
        }
    }
}