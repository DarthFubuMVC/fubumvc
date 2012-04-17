using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Media.Testing
{
    public class ValidStubUrlRegistry : IUrlRegistry
    {

        public string UrlFor(object model, string category = null)
        {
            var url = "http://somewhere.com/" + model.ToString();
            if (category.IsNotEmpty())
            {
                url += "/" + category;
            }

            return url;
        }

        public string UrlFor<TInput>(string categoryOrHttpMethod) where TInput : class
        {
            throw new NotImplementedException();
        }

        public string UrlForNew(Type entityType)
        {
            throw new NotImplementedException();
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
            var url = "http://somewhere.com/" + handlerType.Name + "/" + method.Name;
            if (categoryOrHttpMethodOrHttpMethod.IsNotEmpty())
            {
                url += "/" + categoryOrHttpMethodOrHttpMethod;
            }

            return url;
        }

        public string TemplateFor(object model)
        {
            throw new NotImplementedException();
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            throw new NotImplementedException();
        }

        public string UrlFor(Type modelType, RouteParameters parameters, string categoryOrHttpMethod = null)
        {
            var url = "http://something.com/{0}/{1}".ToFormat(modelType.Name, parameters);

            if (categoryOrHttpMethod.IsNotEmpty())
            {
                url += "/" + categoryOrHttpMethod;
            }

            return url;
        }


        public string UrlFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod)
        {
            return UrlFor(typeof(TController), ReflectionHelper.GetMethod(expression), categoryOrHttpMethod);
        }

        public string UrlForAsset(AssetFolder? folder, string name)
        {
            throw new NotImplementedException();
        }
    }
}