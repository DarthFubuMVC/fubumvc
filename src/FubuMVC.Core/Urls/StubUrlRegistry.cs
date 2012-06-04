using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Urls
{
    /// <summary>
    /// This is just to have a predictable stub for unit testing
    /// </summary>
    public class StubUrlRegistry : IUrlRegistry
    {

        public string UrlFor<TInput>(string categoryOrHttpMethod) where TInput : class
        {
            var url = "url for " + typeof(TInput).Name;

            if (categoryOrHttpMethod.IsNotEmpty())
            {
                url += "/" + categoryOrHttpMethod;
            }

            return url;
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

        public string UrlFor(Type modelType, RouteParameters parameters, string categoryOrHttpMethod)
        {
            return "url for {0}/{1} with parameters {2}".ToFormat(modelType.FullName, categoryOrHttpMethod, parameters);
        }

        public string UrlForAsset(AssetFolder? folder, string name)
        {
            return "url for asset " + name + " in " + folder.ToString();
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod)
        {
            return UrlFor(typeof(TController), ReflectionHelper.GetMethod(expression), categoryOrHttpMethod);
        }

        public string UrlForNew(Type entityType)
        {
            return "url for new " + entityType.FullName;
        }

        public bool HasNewUrl(Type type)
        {
            throw new NotImplementedException();
        }

        public string TemplateFor(object model, string categoryOrHttpMethod)
        {
            throw new NotImplementedException();
        }

        public string UrlFor(Type handlerType, MethodInfo method, string categoryOrHttpMethodOrHttpMethod)
        {
            if (categoryOrHttpMethodOrHttpMethod.IsNotEmpty())
            {
                return "url for {0}.{1}()/{2}".ToFormat(handlerType.FullName, method.Name, categoryOrHttpMethodOrHttpMethod);
            }

            if (method == null)
            {
                return "url for " + handlerType.Name;
            }

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