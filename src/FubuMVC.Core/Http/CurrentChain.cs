using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http
{
    public class CurrentChain : ICurrentChain
    {
        private readonly Stack<BehaviorChain> _chains = new Stack<BehaviorChain>();
        private readonly Lazy<string> _resourceHash;
        private readonly BehaviorChain _originalChain;

        public CurrentChain(BehaviorChain top, IDictionary<string, object> data)
        {
            _chains.Push(top);
            _originalChain = top;

            _resourceHash = new Lazy<string>(() =>
            {
                var dict = new Dictionary<string, string>{
                    {"Pattern", top.Route.Pattern}
                };

                data.OrderBy(x => x.Key).Each(pair =>
                {
                    dict.Add(pair.Key, pair.Value == null ? string.Empty : pair.Value.ToString());
                });

                var name = dict.Select(x => "{0}={1}".ToFormat(x.Key, x.Value)).Join(";");
                return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(name)).Select(b => b.ToString("x2")).Join("");
            });
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
    }
}