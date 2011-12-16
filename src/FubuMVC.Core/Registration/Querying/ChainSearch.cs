using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using System.Linq;

namespace FubuMVC.Core.Registration.Querying
{
    public class ChainSearch
    {
        public Type Type;
        public string CategoryOrHttpMethod = Categories.DEFAULT;
        public CategorySearchMode CategoryMode = CategorySearchMode.Relaxed;
        public TypeSearchMode TypeMode = TypeSearchMode.Any;

        public string Description()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BehaviorChain> FindForCategory(IEnumerable<BehaviorChain> chains)
        {
            if (CategoryMode == CategorySearchMode.Strict)
            {
                return chains.Where(x => x.UrlCategory.Category == (CategoryOrHttpMethod ?? Categories.DEFAULT));
            }

            if (chains.Count() == 1)
            {
                return chains;
            }

            if (CategoryOrHttpMethod == null)
            {
                var candidates = chains.Where(x => x.UrlCategory.Category == Categories.DEFAULT);
                if (candidates.Count() > 0) return candidates;

                return chains.Where(x => x.UrlCategory.Category == null);
            }

            throw new NotImplementedException();
        }
    }
}