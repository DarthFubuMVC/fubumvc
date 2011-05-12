using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{

    public interface IChainForwarder
    { 
        string Category { get; }
        Type InputType { get; }
        ForwardingResult FindChain(IChainResolver resolver, object model);

        
    }

    public class ForwardingResult
    {
        public object RealInput { get; set; }
        public BehaviorChain Chain { get; set; }
    }



    public class ChainForwarder<T> : IChainForwarder
    {
        private readonly Func<T, object> _converter;
        private readonly string _category;

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

        public ForwardingResult FindChain(IChainResolver resolver, object model)
        {
            var input = (T)model;
            var realInput = _converter(input);

            if (realInput == null)
            {
                throw new FubuException(2111, "Chain Forwarder for {0} did not return any value for {1}", typeof(T).FullName, model.ToString());
            }

            // Limitation.  Not respecting the category for now.
            return new ForwardingResult(){
                Chain = resolver.FindUnique(realInput),
                RealInput = realInput
            };
        }
    }
}