using System;
using FubuCore;

namespace FubuMVC.Core.View.Activation
{
    public static class ViewTypeExtensions
    {
        public static Type InputModel(this Type viewType)
        {
            return viewType.FindParameterTypeTo(typeof (IFubuPage<>));
        }
    }
}