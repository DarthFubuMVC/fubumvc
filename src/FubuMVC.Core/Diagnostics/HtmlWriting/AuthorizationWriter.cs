using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    [WannaKill]
    public class AuthorizationWriter
    {
        public static HtmlTag BuildListOfRoles(BehaviorGraph behaviors, Action<BehaviorChain, HtmlTag> buildChainElement)
        {
            var tag = new HtmlTag("ul");
            var collector = new RoleCollector();
            collector.Register(behaviors);
            collector.AllRoleSets().Each(role =>
            {
                var li = tag.Add("li").Text(role.Role);
                var ul = li.Add("ul");
                role.Chains.Each(c => buildChainElement(c, ul.Add("li")));
            });

            return tag;
        }
    }

    [WannaKill]
    public class RoleCollector
    {
        private readonly Cache<string, RoleSet> _roles = new Cache<string, RoleSet>(r => new RoleSet(r));

        public void Register(BehaviorGraph graph)
        {
            graph.Behaviors.Each(chain =>
            {
                chain.Authorization.AllowedRoles().Each(role => _roles[role].Add(chain));
            });
        }

        public IEnumerable<RoleSet> AllRoleSets()
        {
            return _roles.GetAll().OrderBy(x => x.Role);
        }
    }

    [WannaKill]
    public class RoleSet
    {
        private readonly string _role;
        private readonly List<BehaviorChain> _chains = new List<BehaviorChain>();

        public RoleSet(string role)
        {
            _role = role;
        }

        public string Role
        {
            get { return _role; }
        }

        public List<BehaviorChain> Chains
        {
            get { return _chains; }
        }

        public void Add(BehaviorChain chain)
        {
            _chains.Add(chain);
        }
    }
}