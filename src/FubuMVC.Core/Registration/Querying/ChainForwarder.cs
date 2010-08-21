using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{

    public interface IChainForwarder
    { 
        string Category { get; }
        Type InputType { get; }
        BehaviorChain FindChain(IChainResolver resolver, object model);
        string FindUrl(IChainResolver resolver, object model);
    }



    public class ChainForwarder<T> : IChainForwarder
    {
        private readonly Func<T, object> _converter;
        private string _category;

        public ChainForwarder(Func<T, object> converter)
        {
            _converter = converter;
        }

        public ChainForwarder(Func<T, object> converter, string category)
        {
            _converter = converter;
            _category = category;
        }

        public string Category
        {
            get { return _category; }
        }

        public Type InputType
        {
            get { return typeof(T); }
        }

        public BehaviorChain FindChain(IChainResolver resolver, object model)
        {
            var input = (T)model;
            var realInput = _converter(input);

            // Limitation.  Not respecting the category for now.
            return resolver.FindUnique(realInput);
        }

        public string FindUrl(IChainResolver resolver, object model)
        {
            var input = (T)model;
            var realInput = _converter(input);
            var chain = resolver.FindUnique(realInput);

            return chain.Route.CreateUrl(realInput);
        }
    }
}