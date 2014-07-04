using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Util;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration
{
    public class ActionSource : IActionSource, DescribesItself
    {
        private readonly AppliesToExpression _applies = new AppliesToExpression();
        private readonly List<Assembly> _assemblies = new List<Assembly>(); 
        private readonly StringWriter _description = new StringWriter();

        public static bool IsCandidate(MethodInfo method)
        {
            if (method.DeclaringType.Equals(typeof(object))) return false;

            int parameterCount = method.GetParameters() == null ? 0 : method.GetParameters().Length;
            if (parameterCount > 1) return false;

            if (method.GetParameters().Any(x => x.ParameterType.IsSimple())) return false;
            
            bool hasOutput = method.ReturnType != typeof (void);

            if (hasOutput && !method.ReturnType.IsClass) return false;

            

            if (hasOutput) return true;

            return parameterCount == 1;
        }

        private readonly ActionMethodFilter _methodFilters;
        private readonly CompositeFilter<Type> _typeFilters = new CompositeFilter<Type>();
        private readonly CompositeFilter<ActionCall> _callFilters = new CompositeFilter<ActionCall>();

        public ActionSource()
        {
            _description.WriteLine("Public methods that follow the 1 in/1 out, 0 in/ 1 out, or 1 in/ 0 out pattern");

            _methodFilters = new ActionMethodFilter();
        }

        public AppliesToExpression Applies
        {
            get { return _applies; }
        }


        IEnumerable<ActionCall> IActionSource.FindActions(Assembly applicationAssembly)
        {
            var types = _applies.BuildPool(applicationAssembly);
            _assemblies.AddRange(types.Assemblies);

            return types.TypesMatching(_typeFilters.Matches).SelectMany(actionsFromType);
        }

        private IEnumerable<ActionCall> actionsFromType(Type type)
        {
            return type.PublicInstanceMethods()
                .Where(_methodFilters.Matches)
                .Where(IsCandidate)
                .Select(m => buildAction(type, m))
                .Where(_callFilters.Matches);
        }

        protected virtual ActionCall buildAction(Type type, MethodInfo method)
        {
            return new ActionCall(type, method);
        }

        /// <summary>
        /// Find Actions on classes whose name ends on 'Controller'
        /// </summary>
        public void IncludeClassesSuffixedWithController()
        {
            _description.WriteLine("Public classes that are suffixed by 'Controller'");
            IncludeTypesNamed(x => x.EndsWith("Controller"));
        }

        /// <summary>
        /// Find Actions on classes whose name ends with 'Endpoint'
        /// </summary>
        public void IncludeClassesSuffixedWithEndpoint()
        {
            _description.WriteLine("Public class that are suffixed by 'Endpoint' or 'Endpoints'");
            IncludeTypesNamed(x => x.EndsWith("Endpoint", StringComparison.OrdinalIgnoreCase) || x.EndsWith("Endpoints", StringComparison.OrdinalIgnoreCase));
        }

        public void IncludeTypesNamed(Expression<Func<string, bool>> filter)
        {
            _description.WriteLine("Classes that match " + filter.ToString());

            var typeParam = Expression.Parameter(typeof(Type), "type"); // type =>
            var nameProp = Expression.Property(typeParam, "Name");  // type.Name
            var invokeFilter = Expression.Invoke(filter, nameProp); // filter(type.Name)
            var lambda = Expression.Lambda<Func<Type, bool>>(invokeFilter, typeParam); // type => filter(type.Name)

            IncludeTypes(lambda);
        }

        /// <summary>
        /// Find Actions on types that match on the provided filter
        /// </summary>
        public void IncludeTypes(Expression<Func<Type, bool>> filter)
        {
            _description.WriteLine("Public class that match " + filter.ToString());

            _typeFilters.Includes += filter;
        }

        /// <summary>
        /// Find Actions on concrete types assignable to T
        /// </summary>
        public void IncludeTypesImplementing<T>()
        {
            _description.WriteLine("Where types are concrete types that implement the {0} interface".ToFormat(typeof(T).FullName));
            IncludeTypes(type => !type.IsOpenGeneric() && type.IsConcreteTypeOf<T>());
        }

        /// <summary>
        /// Actions that match on the provided filter will be added to the runtime. 
        /// </summary>
        public void IncludeMethods(Expression<Func<MethodInfo, bool>> filter)
        {
            _description.WriteLine("Methods matching " + filter.ToString());
            _methodFilters.Includes += filter;
        }

        /// <summary>
        /// Exclude types that match on the provided filter for finding Actions
        /// </summary>
        public void ExcludeTypes(Expression<Func<Type, bool>> filter)
        {
            _description.WriteLine("Exclude types matching " + filter.ToString());
            _typeFilters.Excludes += filter;
        }

        /// <summary>
        /// Actions that match on the provided filter will NOT be added to the runtime. 
        /// </summary>
        public void ExcludeMethods(Expression<Func<MethodInfo, bool>> filter)
        {
            _description.WriteLine("Exclude methods matching " + filter);
            _methodFilters.Excludes += filter;
        }

        /// <summary>
        /// Ignore any methods that are declared by a super type or interface T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void IgnoreMethodsDeclaredBy<T>()
        {
            _description.WriteLine("Exclude methods declared by type " + typeof(T).FullName);
            _methodFilters.IgnoreMethodsDeclaredBy<T>();
        }

        /// <summary>
        /// Exclude any types that are not concrete
        /// </summary>
        public void ExcludeNonConcreteTypes()
        {
            _description.WriteLine("Excludes non-concrete types");
            _typeFilters.Excludes += type => !type.IsConcrete();
        }

        void DescribesItself.Describe(Description description)
        {
            var list = new BulletList
            {
                Name = "Assemblies"
            };

            _assemblies.Each(assem => {
                list.Children.Add(new Description
                {
                    Title = assem.FullName
                });
            });

            description.Title = "Action Source";
            description.BulletLists.Add(list);
            description.LongDescription = _description.ToString();

        }
    }
}