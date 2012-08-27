using System;
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

        public void Apply(IDictionary<string, string> dictionary)
        {
            dictionary.Add("chain", _currentChain.Current.UniqueId.ToString());

            if (!_currentChain.Current.IsPartialOnly && _currentChain.Current.Route != null)
            {
                if (_currentChain.Current.Route.Input != null && _currentChain.RouteData != null)
                {
                    _currentChain.RouteData.Each(pair => dictionary.Add(pair.Key, (pair.Value ?? string.Empty).ToString()));
                }
            }
        }
    }
}