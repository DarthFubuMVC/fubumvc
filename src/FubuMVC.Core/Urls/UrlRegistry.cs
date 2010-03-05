using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Urls
{
    public class UrlRegistry : IUrlRegistry, IUrlRegistration
    {
        private readonly List<ActionUrl> _actions = new List<ActionUrl>();
        private readonly Cache<Type, List<IModelUrl>> _byType = new Cache<Type, List<IModelUrl>>();
        private readonly List<IModelUrl> _modelUrls = new List<IModelUrl>();
        private readonly Cache<Type, ActionUrl> _news = new Cache<Type, ActionUrl>();

        public UrlRegistry()
        {
            _news.OnMissing =
                type => { throw new FubuException(2101, "No 'New' url registered for type {0}", type.FullName); };

            _byType.OnMissing = type => { return _modelUrls.Where(x => x.InputType == type).ToList(); };
        }

        public void Forward<TInput>(Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            Forward(Categories.DEFAULT, forward);
        }

        public IEnumerable<ActionUrl> Actions { get { return _actions; } }
        public IEnumerable<IModelUrl> ModelUrls { get { return _modelUrls; } }

        public void Add(IEnumerable<ActionUrl> actions)
        {
            _actions.AddRange(actions);
        }


        public void Add(IEnumerable<IModelUrl> models)
        {
            _modelUrls.AddRange(models);
        }

        public void RegisterNew(ActionUrl action, Type type)
        {
            _news[type] = action;
        }

        public void AddAction(ActionUrl action)
        {
            _actions.Add(action);
        }

        public void AddModel(IModelUrl model)
        {
            _modelUrls.Add(model);
        }

        public void Forward<TInput>(Type type, string category, Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            Func<object, string> func = o => forward.Compile()((TInput) o, this);
            var url = new ForwardUrl(type, func, category, forward.ToString());
            AddModel(url);
        }

        public void Forward<TInput>(string category, Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            Forward(typeof (TInput), category, forward);
        }

        public string UrlFor(object model)
        {
            if (model == null) return null;

            Type modelType = model.GetType();
            List<IModelUrl> models = _byType[modelType];

            switch (models.Count)
            {
                case 0:
                    throw new FubuException(2102, "No urls registered for input type {0}", modelType.FullName);

                case 1:
                    return models[0].CreateUrl(model);

                default:
                    IModelUrl defaultModel = models.FirstOrDefault(x => x.Category == Categories.DEFAULT);
                    if (defaultModel != null)
                    {
                        return defaultModel.CreateUrl(model);
                    }
                    throw new FubuException(2103,
                                            "More than one url is registered for {0} and none is marked as the default.\n{1}",
                                            modelType.FullName, listAllModels(models));
            }
        }

        public string UrlFor(object model, string category)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Type modelType = model.GetType();
            IEnumerable<IModelUrl> models = _byType[modelType].Where(x => x.Category == category);

            if (!models.Any())
                throw new FubuException(2104, "No urls are registered for {0}, category {1}", modelType.FullName,
                                        category);
            if (models.Count() > 1)
            {
                throw new FubuException(2105, "More than one url is registered for {0}, category {1}.{2}",
                                        modelType.FullName, category, listAllModels(models));
            }

            return models.Single().CreateUrl(model);
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);

            if (method == null)
            {
                throw new FubuException(2108, "Could not find a method from expression: " + expression);
            }


            Type controllerType = typeof (TController);
            return UrlFor(controllerType, method);
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            ActionUrl actionUrl =
                _actions.FirstOrDefault(x => x.HandlerType == handlerType && x.Method.Name == method.Name);

            if (actionUrl == null)
                throw new FubuException(2106, "No url registered for {0}.{1}()", handlerType.FullName, method.Name);


            return actionUrl.GetUrl(null);
        }

        public string UrlForNew<T>()
        {
            return UrlForNew(typeof (T));
        }

        public string UrlForNew(Type entityType)
        {
            return _news[entityType].GetUrl(null);
        }

        public bool HasNewUrl<T>()
        {
            return HasNewUrl(typeof (T));
        }

        public bool HasNewUrl(Type type)
        {
            return _news.Has(type);
        }

        public string UrlForPropertyUpdate(object model)
        {
            return UrlFor(model, Categories.PROPERTY_EDIT);
        }

        [Obsolete("TEMPORARY HACK")]
        public string UrlForPropertyUpdate(Type type)
        {
            object o = Activator.CreateInstance(type);
            return UrlForPropertyUpdate(o);
        }

        private static string listAllModels(IEnumerable<IModelUrl> models)
        {
            return String.Join("\n", models.Select(m => m.ToString()).ToArray());
        }
    }
}