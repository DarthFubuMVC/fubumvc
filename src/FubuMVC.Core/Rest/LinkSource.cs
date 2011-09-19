using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuCore.Reflection;
using FubuLocalization;
using FubuMVC.Core.Rest.Projections;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public class LinkSource<T> : ILinkSource<T>
    {
        private readonly Func<IValueSource<T>, IUrlRegistry, string> _urlSource;
        private Func<IValueSource<T>, bool> _filter = t => true;
        private readonly IList<Action<SyndicationLink>> _modifications = new List<Action<SyndicationLink>>();

        public LinkSource(Func<IValueSource<T>, IUrlRegistry, string> urlSource)
        {
            _urlSource = urlSource;
        }

        private LinkSource<T> modify(Action<SyndicationLink> modification)
        {
            _modifications.Add(modification);
            return this;
        }

        public LinkSource<T> Rel(string rel)
        {
            return modify(link => link.RelationshipType = rel);
        }

        public LinkSource<T> Title(StringToken title)
        {
            return modify(link => link.Title = title.ToString());
        }

        public LinkSource<T> If(Func<IValueSource<T>, bool> filter)
        {
            _filter = filter;
            return this;
        } 

        public LinkSource<T> IfSubjectMatches(Func<T, bool> filter)
        {
            return If(t => filter((T) t.Subject));
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


        IEnumerable<SyndicationLink> ILinkSource<T>.LinksFor(IValueSource<T> target, IUrlRegistry urls)
        {
            if (!_filter(target))
            {
                yield break;
            }

            var url = _urlSource(target, urls);
            var link = new SyndicationLink(new Uri(url));
            _modifications.Each(x => x(link));

            yield return link;
        }
    }
}