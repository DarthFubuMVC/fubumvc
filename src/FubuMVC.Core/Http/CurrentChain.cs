using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http
{
    public class CurrentChain : ICurrentChain
    {
        private readonly Stack<BehaviorChain> _chains = new Stack<BehaviorChain>();
        private readonly BehaviorChain _originalChain;
        private readonly Lazy<string> _resourceHash;
        private readonly IDictionary<string, object> _routeData;

        public CurrentChain(BehaviorChain top, IDictionary<string, object> data)
        {
            _chains.Push(top);
            _originalChain = top;

            _routeData = data;

            _resourceHash = new Lazy<string>(() =>
            {
                var dict = new Dictionary<string, string>{
                    {"Pattern", top.ToString()}
                };

                data.OrderBy(x => x.Key).Each(
                    pair => { dict.Add(pair.Key, pair.Value == null ? string.Empty : pair.Value.ToString()); });

                return dict.Select(x => "{0}={1}".ToFormat(x.Key, x.Value)).Join(";").ToHash();
            });
        }

        public IDictionary<string, object> RouteData
        {
            get { return _routeData; }
        }

        public BehaviorChain Current
        {
            get { return _chains.Peek(); }
        }

        public BehaviorChain OriginatingChain
        {
            get { return _originalChain; }
        }

        public void Push(BehaviorChain chain)
        {
            _chains.Push(chain);
        }

        public void Pop()
        {
            _chains.Pop();
        }

        public string ResourceHash()
        {
            return _resourceHash.Value;
        }

        public bool IsInPartial()
        {
            return _chains.Count > 1;
        }
    }
}