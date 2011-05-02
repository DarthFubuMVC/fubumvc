using System;

namespace FubuMVC.WebForms
{
    public interface IPartialViewTypeRegistry
    {
        Type GetPartialViewTypeFor<TPartialModel>();
        bool HasPartialViewTypeFor<TPartialModel>();
        void Register(Type modelType, Type expression);
    }
}