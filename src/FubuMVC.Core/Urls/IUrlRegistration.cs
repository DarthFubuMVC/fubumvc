using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Urls
{
    public class ForwardUrl : IModelUrl
    {
        private readonly string _category;
        private readonly string _description;
        private readonly Func<object, string> _forward;
        private readonly Type _inputType;

        public ForwardUrl(Type inputType, Func<object, string> forward, string category, string description)
        {
            _inputType = inputType;
            _forward = forward;
            _category = category;
            _description = description;
        }

        public string CreateUrl(object input)
        {
            return _forward(input);
        }

        public void RootUrlAt(string baseUrl)
        {
            // no op
        }

        public string Category { get { return _category; } }

        public Type InputType { get { return _inputType; } }

        public override string ToString()
        {
            return "[{0}] {1} --> {2}".ToFormat(_category, _inputType, _description);
        }
    }

    public interface IUrlRegistration
    {
        IEnumerable<ActionUrl> Actions { get; }
        IEnumerable<IModelUrl> ModelUrls { get; }
        void RegisterNew(ActionUrl action, Type type);
        void RegisterNew(ActionCall call, Type type);

        void AddAction(ActionUrl action);
        void AddModel(IModelUrl model);
        void MapModelTypes(Func<object, Type> typeMap);
        void Forward<TInput>(Type type, string category, Expression<Func<TInput, IUrlRegistry, string>> forward);
        void Forward<TInput>(string category, Expression<Func<TInput, IUrlRegistry, string>> forward);
        void Forward<TInput>(Expression<Func<TInput, IUrlRegistry, string>> forward);
    }
}