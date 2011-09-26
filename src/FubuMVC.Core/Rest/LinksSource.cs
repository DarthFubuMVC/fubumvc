using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{
    public class LinksSource<T> : ILinkSource<T>
    {
        private readonly IList<ILinkSource<T>> _sources = new List<ILinkSource<T>>();

        IEnumerable<Link> ILinkSource<T>.LinksFor(IValues<T> target, IUrlRegistry urls)
        {
            return _sources.SelectMany(x => x.LinksFor(target, urls));
        }

        public LinkSource<T> ToSubject()
        {
            return To(v => v);
        }

        public LinkSource<T> ToSubject(Expression<Func<T, object>> identifierProperty)
        {
            var accessor = identifierProperty.ToAccessor();

            return To((values, urls) =>
            {
                var parameters = new RouteParameters<T>();
                parameters[accessor.Name] = values.ValueFor(accessor).ToString();

                return urls.UrlFor(typeof (T), parameters);
            });
        }

        public LinkSource<T> To(Func<T, object> subject)
        {
            return To((values, urls) =>
            {
                var urlSubject = subject(values.Subject);
                return urls.UrlFor(urlSubject);
            });
        }

        public LinkSource<T> To(Func<IValues<T>, IUrlRegistry, string> urlSource)
        {
            var source = new LinkSource<T>(urlSource);
            _sources.Add(source);

            return source;
        }
    }
}