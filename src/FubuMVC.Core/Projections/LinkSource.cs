using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuMVC.Core.Projections
{
    public class LinkSource<T> : ILinkSource
    {
        private readonly Func<IProjectionTarget, string> _urlSource;
        private Func<IProjectionTarget, bool> _filter = t => true;
        private readonly IList<Action<SyndicationLink>> _modifications = new List<Action<SyndicationLink>>();

        public LinkSource(Func<IProjectionTarget, string> urlSource)
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

        public LinkSource<T> If(Func<IProjectionTarget, bool> filter)
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


        IEnumerable<SyndicationLink> ILinkSource.LinksFor(IProjectionTarget target)
        {
            if (!_filter(target))
            {
                yield break;
            }

            var url = _urlSource(target);
            var link = new SyndicationLink(new Uri(url));
            _modifications.Each(x => x(link));

            yield return link;
        }
    }
}