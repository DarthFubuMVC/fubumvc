using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuCore;
using FubuMVC.Core.Registration.Routes;
using FubuCore.Reflection;
using System.Linq;

namespace FubuMVC.Core.Projections
{
    public class LinksSource<T> : ILinkSource
    {
        private readonly IList<ILinkSource> _sources = new List<ILinkSource>();

        public LinkSource<T> LinkToSubject()
        {
            return LinkTo(x => x.Urls.UrlFor(x.Subject));    
        }

        public LinkSource<T> LinkToSubject(Expression<Func<T, object>> identifierProperty)
        {
            var accessor = identifierProperty.ToAccessor();

            return LinkTo(x =>
            {
                var parameters = new RouteParameters<T>();
                parameters[accessor.Name] = x.ValueFor(accessor).ToString();

                return x.Urls.UrlFor(typeof (T), parameters);
            });
        }

        public LinkSource<T> LinkTo(Func<T, object> subject)
        {
            return LinkTo(x =>
            {
                var urlSubject = subject(x.Subject.As<T>());
                return x.Urls.UrlFor(urlSubject);
            });
        }

        public LinkSource<T> LinkTo(Func<IProjectionTarget, string> urlSource)
        {
            var source = new LinkSource<T>(urlSource);
            _sources.Add(source);

            return source;
        }

        IEnumerable<SyndicationLink> ILinkSource.LinksFor(IProjectionTarget target)
        {
            return _sources.SelectMany(x => x.LinksFor(target));
        }
    }
}