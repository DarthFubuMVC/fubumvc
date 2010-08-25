using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuMVC.Core.Security
{
    public interface IAuthorizationPreviewService
    {
        bool IsAuthorized(object model);
        bool IsAuthorized(object model, string category);
        bool IsAuthorized<TController>(Expression<Action<TController>> expression);

        bool IsAuthorizedForNew<T>();
        bool IsAuthorizedForNew(Type entityType);

        bool IsAuthorizedForPropertyUpdate(object model);
        bool IsAuthorizedForPropertyUpdate(Type type);

        bool IsAuthorized(Type handlerType, MethodInfo method);
    }
}