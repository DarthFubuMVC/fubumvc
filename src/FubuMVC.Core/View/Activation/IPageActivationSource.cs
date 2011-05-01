using System;
using System.Collections.Generic;

namespace FubuMVC.Core.View.Activation
{
    public interface IPageActivationSource
    {
        IEnumerable<IPageActivationAction> ActionsFor(Type viewType);
    }
}