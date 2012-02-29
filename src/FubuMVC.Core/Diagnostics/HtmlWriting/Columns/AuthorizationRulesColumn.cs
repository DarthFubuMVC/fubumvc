using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public class AuthorizationRulesColumn : IColumn
    {
        private readonly IBehaviorFactory _factory;

        public AuthorizationRulesColumn(IBehaviorFactory factory)
        {
            _factory = factory;
        }

        public string Header()
        {
            return "Authorization";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell)
        {
            if (!chain.Authorization.HasRules())
            {
                cell.Text("  [None]  ");
                return;
            }

            cell.Add("ul", ul =>
            {
                var authorizor = _factory.AuthorizorFor(chain.UniqueId);
                authorizor.RulesDescriptions().Each(rule => { ul.Add("li").Text(rule); });
            });
        }

        public string Text(BehaviorChain chain)
        {
            if (!chain.Authorization.HasRules())
            {
                return "-- None --";
            }

            var authorizor = _factory.AuthorizorFor(chain.UniqueId);
            return authorizor.RulesDescriptions().Join("; ");
        }
    }
}