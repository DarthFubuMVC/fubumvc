using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Registration.Querying
{
    public class ChainSearch
    {
        public static ChainSearch ByUniqueInputType(Type modelType, string categoryOrHttpMethod = null)
        {
            return new ChainSearch
            {
                Type = modelType,
                TypeMode = TypeSearchMode.Any,
                CategoryOrHttpMethod = categoryOrHttpMethod
            }; 
        }

        public static ChainSearch ForMethod(Type handlerType, MethodInfo method, string categoryOrHttpMethod = null)
        {
            var search = new ChainSearch
            {
                Type = handlerType,
                TypeMode = TypeSearchMode.HandlerOnly,
                MethodName = method == null ? null : method.Name,
                CategoryOrHttpMethod = categoryOrHttpMethod
            };

            if (method == null)
            {
                search.TypeMode = TypeSearchMode.Any;
            }

            return search;
        }

        public static ChainSearch ForMethod<T>(Expression<Action<T>> expression,
                                               string categoryOrHttpMethod = null)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            return ForMethod(typeof (T), method, categoryOrHttpMethod);
        }

        public Type Type;
        public string CategoryOrHttpMethod = Categories.DEFAULT;
        public CategorySearchMode CategoryMode = CategorySearchMode.Relaxed;
        public TypeSearchMode TypeMode = TypeSearchMode.Any;
        public string MethodName;

        public override string ToString()
        {
            return string.Format("Type: {0}, CategoryOrHttpMethod: {1}, CategoryMode: {2}, TypeMode: {3}, MethodName: {4}", Type.FullName, CategoryOrHttpMethod ?? "None", CategoryMode, TypeMode, MethodName);
        }

        public bool Equals(ChainSearch other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.CategoryOrHttpMethod, CategoryOrHttpMethod) && Equals(other.CategoryMode, CategoryMode) && Equals(other.TypeMode, TypeMode) && Equals(other.MethodName, MethodName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ChainSearch)) return false;
            return Equals((ChainSearch) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Type != null ? Type.GetHashCode() : 0);
                result = (result*397) ^ (CategoryOrHttpMethod != null ? CategoryOrHttpMethod.GetHashCode() : 0);
                result = (result*397) ^ CategoryMode.GetHashCode();
                result = (result*397) ^ TypeMode.GetHashCode();
                result = (result*397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
                return result;
            }
        }


        public IEnumerable<BehaviorChain> FindCandidates(BehaviorGraph graph)
        {
            foreach (var level in FindCandidatesByType(graph))
            {
                var candidates = FindForCategory(level);
                if (candidates.Any()) return candidates;
            }

            return new BehaviorChain[0];
        }


        public IEnumerable<IEnumerable<BehaviorChain>> FindCandidatesByType(BehaviorGraph graph)
        {
            Func<IEnumerable<BehaviorChain>, IEnumerable<BehaviorChain>> methodFilter = chains => chains;
            if (MethodName.IsNotEmpty())
            {
                methodFilter = chains => chains.Where(c => c.Calls.Any(x => x.Method.Name == MethodName));
            }


            if (TypeMode == TypeSearchMode.Any || TypeMode == TypeSearchMode.InputModelOnly)
            {
                // TODO -- it's right here.  Need to use anything that has input
                yield return methodFilter(graph.Behaviors.Where(chain => chain.InputType() == Type));
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

                return chains.Where(x => x.Category.IsEmpty());
            }
            else
            {
                return chains.Where(x => x.MatchesCategoryOrHttpMethod(CategoryOrHttpMethod));
            }

            
        }
    }
}