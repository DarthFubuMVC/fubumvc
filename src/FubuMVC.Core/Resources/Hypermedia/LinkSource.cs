using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Hypermedia
{
    public class LinkSource<T> : ILinkSource<T>
    {
        private readonly LinkExpression _linkExpression = new LinkExpression();
        private readonly Func<IValues<T>, IUrlRegistry, string> _urlSource;
        private Func<IValues<T>, bool> _filter = t => true;

        public LinkSource(Func<IValues<T>, IUrlRegistry, string> urlSource)
        {
            _urlSource = urlSource;
        }

        IEnumerable<Link> ILinkSource<T>.LinksFor(IValues<T> target, IUrlRegistry urls)
        {
            if (!_filter(target))
            {
                yield break;
            }

            var url = _urlSource(target, urls);
            var link = new Link(url);
            _linkExpression.As<ILinkModifier>().Modify(link);

            yield return link;
        }


        public LinkSource<T> Rel(string rel)
        {
            _linkExpression.Rel(rel);
            return this;
        }

        public LinkSource<T> Title(object title)
        {
            _linkExpression.Title(title);
            return this;
        }

        public LinkSource<T> If(Func<IValues<T>, bool> filter)
        {
            _filter = filter;
            return this;
        }

        public LinkSource<T> IfSubjectMatches(Func<T, bool> filter)
        {
            return If(t => filter(t.Subject));
        }

        public LinkSource<T> IfEquals(Expression<Func<T, object>> property, object value)
        {
            var accessor = property.ToAccessor();

            return If(t =>
            {
                var subjectValue = t.ValueFor(accessor);
                return value.Equals(subjectValue);
            });
        }

        public LinkSource<T> IfNotEquals(Expression<Func<T, object>> property, object value)
        {
            var accessor = property.ToAccessor();

            return If(t =>
            {
                var subjectValue = t.ValueFor(accessor);
                return !value.Equals(subjectValue);
            });
        }
    }
}