using System;
using System.Linq.Expressions;
using System.Reflection;

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

        // Not sure these two methods won't get axed
        string UrlForPropertyUpdate(object model);
        string UrlForPropertyUpdate(Type type);

        string UrlFor(Type handlerType, MethodInfo method);
    }
}