using Microsoft.Practices.ServiceLocation;

namespace FubuCore
{
    public interface IDisplayFormatter
    {
        string GetDisplay(GetStringRequest request);
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
    }
}