using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuLocalization;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public class LinkSource<T> : ILinkSource<T>
    {
        private readonly IList<Action<Link>> _modifications = new List<Action<Link>>();
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
            var link = new Link{
                Url = url
            };
            _modifications.Each(x => x(link));

            yield return link;
        }

        private LinkSource<T> modify(Action<Link> modification)
        {
            _modifications.Add(modification);
            return this;
        }

        public LinkSource<T> Rel(string rel)
        {
            return modify(link => link.Rel = rel);
        }

        public LinkSource<T> Title(StringToken title)
        {
            return modify(link => link.Title = title.ToString());
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