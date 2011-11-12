using System;
using System.Collections.Generic;

namespace FubuMVC.Core.View.Activation
{
    /// <summary>
    /// Provides instances that implement <see cref="IPageActivationAction"/>
    /// for a given view type.
    /// </summary>
    public interface IPageActivationSource
    {
        IEnumerable<IPageActivationAction> ActionsFor(Type viewType);
    }
}