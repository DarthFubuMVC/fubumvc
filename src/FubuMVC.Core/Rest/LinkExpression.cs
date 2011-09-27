using System;
using System.Collections.Generic;
using FubuLocalization;

namespace FubuMVC.Core.Rest
{
    public class LinkExpression : ILinkModifier
    {
        private readonly IList<Action<Link>> _modifications = new List<Action<Link>>();
    
        private Action<Link> modify
        {
            set
            {
                _modifications.Add(value);
            }
        }


        public LinkExpression Rel(string rel)
        {
            modify = link => link.Rel = rel;
            return this;
        }

        public LinkExpression Title(StringToken title)
        {
            modify = link => link.Title = title.ToString();
            return this;
        }

        public LinkExpression ContentType(string mimeType)
        {
            modify = link => link.ContentType = mimeType;
            return this;
        }

        // TODO -- use the forthcoming FubuCore extension method
        void ILinkModifier.Modify(Link link)
        {
            _modifications.Each(x => x(link));
        }
    }
}