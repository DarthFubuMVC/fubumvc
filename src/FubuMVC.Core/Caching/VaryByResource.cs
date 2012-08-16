using System.Collections.Generic;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public class VaryByResource : IVaryBy
    {
        private readonly ICurrentChain _currentChain;

        public VaryByResource(ICurrentChain currentChain)
        {
            _currentChain = currentChain;
        }

        public IDictionary<string, string> Values()
        {
            var dict = new Dictionary<string, string>{
                {"chain", _currentChain.Current.UniqueId.ToString()}
            };

            if (!_currentChain.Current.IsPartialOnly && _currentChain.Current.Route != null)
            {
                if (_currentChain.Current.Route.Input != null && _currentChain.RouteData != null)
                {
                    _currentChain.RouteData.Each(pair => dict.Add(pair.Key, (pair.Value ?? string.Empty).ToString()));
                }
            }

            return dict;
        }
    }
}