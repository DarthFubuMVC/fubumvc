using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Registration.Querying
{
    public class ChainSearch
    {
        public Type Type;
        public string CategoryOrHttpMethod = Categories.DEFAULT;
        public CategorySearchMode CategoryMode = CategorySearchMode.Relaxed;
        public TypeSearchMode TypeMode = TypeSearchMode.Any;
        public string MethodName;

        public string Description()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<BehaviorChain>> FindCandidates(BehaviorGraph graph)
        {
            Func<IEnumerable<BehaviorChain>, IEnumerable<BehaviorChain>> methodFilter = chains => chains;
            if (MethodName.IsNotEmpty())
            {
                methodFilter = chains => chains.Where(c => c.Calls.Any(x => x.Method.Name == MethodName));
            }


            if (TypeMode == TypeSearchMode.Any || TypeMode == TypeSearchMode.InputModelOnly)
            {
                yield return methodFilter(graph.ChainsFor(Type));
            }

            if (TypeMode == TypeSearchMode.Any || TypeMode == TypeSearchMode.HandlerOnly)
            {
                yield return methodFilter(graph.Behaviors.Where(x => x.Calls.Any(c => c.HandlerType == Type)));
            }
        }

        public IEnumerable<BehaviorChain> FindForCategory(IEnumerable<BehaviorChain> chains)
        {
            if (CategoryMode == CategorySearchMode.Strict)
            {
                var category = CategoryOrHttpMethod ?? Categories.DEFAULT;
                return chains.Where(x => x.MatchesCategoryOrHttpMethod(category));
            }

            if (chains.Count() == 1)
            {
                return chains;
            }

            if (CategoryOrHttpMethod == null)
            {
                var candidates = chains.Where(x => x.MatchesCategoryOrHttpMethod(Categories.DEFAULT));
                if (candidates.Count() > 0) return candidates;

                return chains.Where(x => x.UrlCategory.Category == null);
            }

            return chains.Where(x => x.MatchesCategoryOrHttpMethod(CategoryOrHttpMethod));
        }
    }
}