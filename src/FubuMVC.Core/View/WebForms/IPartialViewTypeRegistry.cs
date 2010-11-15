using System;

namespace FubuMVC.Core.View.WebForms
{
    public interface IPartialViewTypeRegistry
    {
        Type GetPartialViewTypeFor<TPartialModel>();
        bool HasPartialViewTypeFor<TPartialModel>();
        void Register(Type modelType, Type expression);
    }
}