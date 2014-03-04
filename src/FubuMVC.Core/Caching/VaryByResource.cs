using System;
using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

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
            dictionary.Add("chain", _currentChain.Current.ToString());

            // TODO -- do this w/ polymorphism
            if (_currentChain.RouteData != null && _currentChain.Current is RoutedChain)
            {
                _currentChain.RouteData.Each(pair => dictionary.Add(pair.Key, (pair.Value ?? string.Empty).ToString()));
            }
        }
    }
}