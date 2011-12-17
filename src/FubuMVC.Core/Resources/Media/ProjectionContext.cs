using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Resources.Media
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

        public T Subject
        {
            get { return _values.Subject; }
        }

        public object ValueFor(Accessor accessor)
        {
            return _values.ValueFor(accessor);
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
    }
}