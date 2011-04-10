using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public class AuthorizationRulesColumn : IColumn
    {
        private readonly IServiceLocator _locator;

        public AuthorizationRulesColumn(IServiceLocator locator)
        {
            _locator = locator;
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
                var authorizor = _locator.GetInstance<IEndPointAuthorizor>(chain.UniqueId.ToString());
                authorizor.RulesDescriptions().Each(rule =>
                {
                    ul.Add("li").Text(rule);
                });
            });
        }

        public string Text(BehaviorChain chain)
        {
            if (!chain.Authorization.HasRules())
            {
                return "-- None --";
            }

            var authorizor = _locator.GetInstance<IEndPointAuthorizor>(chain.UniqueId.ToString());
            return authorizor.RulesDescriptions().Join("; ");
        }
    }
}