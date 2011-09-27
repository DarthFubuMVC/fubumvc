using System;
using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public interface ILinkCreator
    {
        Link CreateLink(IUrlRegistry urls);
    }

    public class LinkExpression : ILinkModifier, ILinkCreator
    {
        private readonly Func<IUrlRegistry, string> _source;
        private readonly IList<Action<Link>> _modifications = new List<Action<Link>>();

        public LinkExpression()
        {
        }

        public LinkExpression(Func<IUrlRegistry, string> source)
        {
            _source = source;
        }

        public Link CreateLink(IUrlRegistry urls)
        {
            var url = _source(urls);
            var link = new Link(url);
            _modifications.Each(x => x(link));

            return link;
        }

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