using System;
using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core.Urls;

namespace FubuMVC.Media
{
    public interface ILinkCreator
    {
        Link CreateLink(IUrlRegistry urls);
    }

    public class LinkExpression : ILinkModifier, ILinkCreator
    {
        private readonly IList<Action<Link>> _modifications = new List<Action<Link>>();
        private readonly Func<IUrlRegistry, string> _source;

        public LinkExpression()
        {
        }

        public LinkExpression(Func<IUrlRegistry, string> source)
        {
            _source = source;
        }

        private Action<Link> modify
        {
            set { _modifications.Add(value); }
        }

        public Link CreateLink(IUrlRegistry urls)
        {
            var url = _source(urls);
            var link = new Link(url);
            _modifications.Each(x => x(link));

            return link;
        }

        void ILinkModifier.Modify(Link link)
        {
            _modifications.Each(x => x(link));
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
    }
}