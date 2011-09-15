using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public class LinksSource<T> : ILinkSource<T>
    {
        private readonly IList<ILinkSource<T>> _sources = new List<ILinkSource<T>>();

        IEnumerable<SyndicationLink> ILinkSource<T>.LinksFor(IValueSource<T> target, IUrlRegistry urls)
        {
            return _sources.SelectMany(x => x.LinksFor(target, urls));
        }

        public LinkSource<T> LinkToSubject()
        {
            return LinkTo(v => v);
        }

        public LinkSource<T> LinkToSubject(Expression<Func<T, object>> identifierProperty)
        {
            var accessor = identifierProperty.ToAccessor();

            return LinkTo((values, urls) =>
            {
                var parameters = new RouteParameters<T>();
                parameters[accessor.Name] = values.ValueFor(accessor).ToString();

                return urls.UrlFor(typeof (T), parameters);
            });
        }

        public LinkSource<T> LinkTo(Func<T, object> subject)
        {
            return LinkTo((values, urls) =>
            {
                var urlSubject = subject(values.Subject);
                return urls.UrlFor(urlSubject);
            });
        }

        public LinkSource<T> LinkTo(Func<IValueSource<T>, IUrlRegistry, string> urlSource)
        {
            var source = new LinkSource<T>(urlSource);
            _sources.Add(source);

            return source;
        }
    }
}