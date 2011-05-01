using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.View.Activation
{
    public class SetPageModelActivationSource : IPageActivationSource
    {
        public IEnumerable<IPageActivationAction> ActionsFor(Type viewType)
        {
            var inputType = viewType.FindParameterTypeTo(typeof (IFubuPage<>));
            if (inputType == null)
            {
                yield break;
            }

            yield return typeof (SetPageModelActivationAction<>).CloseAndBuildAs<IPageActivationAction>(inputType);
        }

        
    }
}