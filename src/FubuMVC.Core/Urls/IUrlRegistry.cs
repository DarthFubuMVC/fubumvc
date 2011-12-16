using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Urls
{
    public interface IUrlRegistry
    {
        string UrlFor(object model, string category = null);
        
        string UrlFor<TInput>() where TInput : class, new();
        

        string UrlFor(Type handlerType, MethodInfo method);
        string UrlFor<TController>(Expression<Action<TController>> expression);

        string UrlForNew(Type entityType);

        bool HasNewUrl(Type type);

        
        string TemplateFor(object model);
        string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new();


        string UrlFor(Type modelType, RouteParameters parameters);
        string UrlFor(Type modelType, string category, RouteParameters parameters);

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

        [Obsolete("TEMPORARY HACK")]
        public static string UrlForPropertyUpdate(this IUrlRegistry registry, Type type)
        {
            var o = Activator.CreateInstance(type);
            return registry.UrlForPropertyUpdate(o);
        }

        public static string UrlFor<TInput>(this IUrlRegistry registry) where TInput : class, new()
        {
            return registry.UrlFor(new TInput());
        }

        public static string UrlFor<TInput>(this IUrlRegistry registry, RouteParameters parameters)
        {
            return registry.UrlFor(typeof(TInput), parameters);
        }

        public static string UrlFor<TInput>(this IUrlRegistry urls, string category, RouteParameters parameters)
        {
            return urls.UrlFor(typeof(TInput), category, parameters);
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

            return registry.UrlFor(typeof(T), category, parameters);
        }
    }

    // This is just to have a predictable stub for unit testing
    public class StubUrlRegistry : IUrlRegistry
    {

        public string UrlFor<TInput>() where TInput : class, new()
        {
            return "url for " + new TInput();
        }

        public string UrlFor(object model, string category = null)
        {
            if (category.IsEmpty())
            {
                return "url for {0}".ToFormat(model);
            }

            return "url for {0}, category {1}".ToFormat(model, category);
        }

        public string UrlFor(Type modelType, RouteParameters parameters)
        {
            return "url for {0} with parameters {1}".ToFormat(modelType.FullName, parameters);
        }

        public string UrlFor(Type modelType, string category, RouteParameters parameters)
        {
            return "url for {0}/{1} with parameters {2}".ToFormat(modelType.FullName, category, parameters);
        }

        public string UrlForAsset(AssetFolder? folder, string name)
        {
            return "url for asset " + name + " in " + folder.ToString();
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            return UrlFor(typeof(TController), ReflectionHelper.GetMethod(expression));
        }

        public string UrlForNew(Type entityType)
        {
            return "url for new " + entityType.FullName;
        }

        public bool HasNewUrl(Type type)
        {
            throw new NotImplementedException();
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            return "url for {0}.{1}()".ToFormat(handlerType.FullName, method.Name);
        }

        public string TemplateFor(object model)
        {
            throw new NotImplementedException();
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            throw new NotImplementedException();
        }
    }
}