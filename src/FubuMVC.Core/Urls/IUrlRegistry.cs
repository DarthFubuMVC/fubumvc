using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore;

namespace FubuMVC.Core.Urls
{
    // This service is injected into your IoC tool of choice as a singleton
    // to give you access to url's in a type safe way
    // Please note that this implementation in no way, shape, or form
    // locks you into a rigid url structure
    public interface IUrlRegistry
    {
        string UrlFor(object model);
        string UrlFor(object model, string category);
        string UrlFor<TController>(Expression<Action<TController>> expression);

        string UrlForNew<T>();
        string UrlForNew(Type entityType);
        bool HasNewUrl<T>();
        bool HasNewUrl(Type type);


        // Not sure these two methods won't get axed
        string UrlForPropertyUpdate(object model);
        string UrlForPropertyUpdate(Type type);

        string UrlFor(Type handlerType, MethodInfo method);
    }

    // This is just to have a predictable stub for unit testing
    public class StubUrlRegistry : IUrlRegistry
    {
        public string UrlFor(object model)
        {
            return "url for " + model.ToString();
        }

        public string UrlFor(object model, string category)
        {
            return UrlFor(model) + ", category=" + category;
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            return "url for " + typeof (TController).FullName + "." + ReflectionHelper.GetMethod(expression).Name + "()";
        }

        public string UrlForNew<T>()
        {
            return "url for new " + typeof (T).FullName;
        }

        public string UrlForNew(Type entityType)
        {
            return "url for new " + entityType.FullName;
        }

        public bool HasNewUrl<T>()
        {
            throw new NotImplementedException();
        }

        public bool HasNewUrl(Type type)
        {
            throw new NotImplementedException();
        }

        public string UrlForPropertyUpdate(object model)
        {
            return "url for property update: " + model.ToString();
        }

        public string UrlForPropertyUpdate(Type type)
        {
            return "url for property update: " + type.FullName;
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            return "url for {0}.{1}()".ToFormat(handlerType.FullName, method.Name);
        }
    }

}