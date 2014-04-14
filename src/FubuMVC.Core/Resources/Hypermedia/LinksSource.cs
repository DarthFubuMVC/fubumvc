using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Hypermedia
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

        /// <summary>
        /// Uses a RouteParameter<TInputType> object with the same named properties
        /// as the T type to construct a Url.  Very useful when the value stream
        /// you're using doesn't have the actual target type (i.e. NH projections)
        /// </summary>
        /// <typeparam name="TInputType"></typeparam>
        /// <param name="properties"></param>
        /// <returns></returns>
        public LinkSource<T> ToInput<TInputType>(params Expression<Func<T, object>>[] properties)
        {
            var accessors = properties.Select(x => x.ToAccessor());

            return To((values, urls) =>
            {
                var parameters = new RouteParameters<TInputType>();
                accessors.Each(a =>
                {
                    parameters[a.Name] = values.ValueFor(a).ToString();
                });

                return urls.UrlFor(parameters);
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