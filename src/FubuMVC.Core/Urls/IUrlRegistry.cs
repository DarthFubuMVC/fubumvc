using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Urls
{
    public interface IUrlRegistry
    {
        string UrlFor(object model, string categoryOrHttpMethod = null);
        
        string UrlFor<T>(string categoryOrHttpMethod = null) where T : class;
        
        string UrlFor(Type handlerType, MethodInfo method = null, string categoryOrHttpMethodOrHttpMethod = null);
        string UrlFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod = null);

        string UrlForNew(Type entityType);

        bool HasNewUrl(Type type);

        string TemplateFor(object model, string categoryOrHttpMethod = null);
        string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new();

        string UrlFor(Type modelType, RouteParameters parameters, string categoryOrHttpMethod = null);

        string UrlForAsset(AssetFolder? folder, string name);
    }

    public static class UrlRegistryExtensions
    {


        public static string UrlForNew<T>(this IUrlRegistry registry)
        {
            return registry.UrlForNew(typeof (T));
        }

        public static bool HasNewUrl<T>(this IUrlRegistry registry)
        {
            return registry.HasNewUrl(typeof(T));
        }

        [Obsolete("This is an ancient Dovetail hack.  Getting eliminated whenever the DT guys say it's okay")]
        public static string UrlForPropertyUpdate(this IUrlRegistry registry, object model)
        {
            return registry.UrlFor(model, Categories.PROPERTY_EDIT);
        }

        [Obsolete("This is an ancient Dovetail hack.  Getting eliminated whenever the DT guys say it's okay")]
        public static string UrlForPropertyUpdate(this IUrlRegistry registry, Type type)
        {
            var o = Activator.CreateInstance(type);
            return registry.UrlForPropertyUpdate(o);
        }

        public static string UrlFor<TInput>(this IUrlRegistry registry, RouteParameters parameters)
        {
            return registry.UrlFor(typeof(TInput), parameters);
        }

        public static string UrlFor<TInput>(this IUrlRegistry urls, string category, RouteParameters parameters)
        {
            return urls.UrlFor(typeof(TInput), parameters, category);
        }

        public static string UrlFor<T>(this IUrlRegistry registry, Action<RouteParameters<T>> configure)
        {
            var parameters = new RouteParameters<T>();
            configure(parameters);

            return registry.UrlFor<T>(parameters);
        }

        public static string UrlFor<T>(this IUrlRegistry registry, string category, Action<RouteParameters<T>> configure)
        {
            var parameters = new RouteParameters<T>();
            configure(parameters);

            return registry.UrlFor(typeof(T), parameters, category);
        }
    }
}