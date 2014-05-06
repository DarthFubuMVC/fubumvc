using System;
using FubuCore;
using FubuCore.Formatting;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Projections
{
    public class ProjectionContext<T> : IProjectionContext<T>
    {
        private readonly IServiceLocator _services;
        private readonly IValues<T> _values;
        private readonly Lazy<IUrlRegistry> _urls;
        private readonly Lazy<IDisplayFormatter> _formatter;

        public ProjectionContext(IServiceLocator services, IValues<T> values)
        {
            _services = services;
            _values = values;

            _urls = new Lazy<IUrlRegistry>(() => services.GetInstance<IUrlRegistry>());
            _formatter = new Lazy<IDisplayFormatter>(() => services.GetInstance<IDisplayFormatter>());
        }

        public ProjectionContext(IServiceLocator services, T subject) : this(services, new SimpleValues<T>(subject))
        {
        }

        private ProjectionContext(IServiceLocator services, IValues<T> values, Lazy<IUrlRegistry> urls, Lazy<IDisplayFormatter> formatter)
        {
            _services = services;
            _values = values;
            _urls = urls;
            _formatter = formatter;
        }

        public IValues<T> Values
        {
            get { return _values; }
        }

        public T Subject
        {
            get { return _values.Subject; }
        }

        public TService Service<TService>()
        {
            return _services.GetInstance<TService>();
        }

        public IUrlRegistry Urls
        {
            get { return _urls.Value; }
        }

        public IDisplayFormatter Formatter
        {
            get { return _formatter.Value; }
        }

        public IProjectionContext<TChild> ContextFor<TChild>(TChild child)
        {
            return new ProjectionContext<TChild>(_services, new SimpleValues<TChild>(child), _urls, _formatter);
        }
    }
}