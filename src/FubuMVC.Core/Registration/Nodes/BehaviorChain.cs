using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    ///   BehaviorChain is a configuration model for a single endpoint in a 
    ///   FubuMVC system.  Models route information, the behaviors, and 
    ///   authorization rules
    ///   system
    /// </summary>
    public class BehaviorChain : Chain<BehaviorNode, BehaviorChain>, IRegisterable, IContainerModel
    {
        public const string NoTracing = "NoTracing";

        private readonly IList<IBehaviorInvocationFilter> _filters = new List<IBehaviorInvocationFilter>();
        private readonly IList<IRouteDefinition> _additionalRoutes = new List<IRouteDefinition>(); 
        private readonly Lazy<InputNode> _input;
        private Lazy<OutputNode> _output;
        private IRouteDefinition _route;

        public BehaviorChain()
        {
            Authorization = new AuthorizationNode();
            UrlCategory = new UrlCategory();

            _output = new Lazy<OutputNode>(() =>
            {
                var outputType = ResourceType();
                if (outputType == null || outputType == typeof (void))
                    throw new InvalidOperationException(
                        "Cannot use the OutputNode if the BehaviorChain does not have at least one Action with output");

                return new OutputNode(outputType);
            });

            _input = new Lazy<InputNode>(() =>
            {
                var inputType = InputType();
                if (inputType == null)
                    throw new InvalidOperationException(
                        "Cannot use the InputNode if the BehaviorChain does not have at least one behavior that requires an input type");

                return new InputNode(inputType);
            });
        }

        public bool IsAsynchronous()
        {
            return Calls.Any(x => x.IsAsync);
        }

        public OutputNode Output
        {
            get { return _output.Value; }
        }

        public InputNode Input
        {
            get { return _input.Value; }
        }

        /// <summary>
        ///   Ordered list of IBehaviorInvocationFilter's that can be used
        ///   to apply guard conditions at runtime *before* the behaviors
        ///   are created
        /// </summary>
        public IEnumerable<IBehaviorInvocationFilter> Filters
        {
            get { return _filters; }
        }

        public void AddFilter(IBehaviorInvocationFilter filter)
        {
            Trace(new FilterAdded(filter));
            _filters.Add(filter);
        }

        public Guid UniqueId
        {
            get { return Top == null ? Guid.Empty : Top.UniqueId; }
        }

        /// <summary>
        ///   Marks what package or FubuRegistry created this BehaviorChain
        ///   for the sake of diagnostics
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        ///   All the ActionCall nodes in this chain
        /// </summary>
        public IEnumerable<ActionCall> Calls
        {
            get { return this.OfType<ActionCall>(); }
        }

        /// <summary>
        ///   All the Output nodes in this chain
        /// </summary>
        public IEnumerable<BehaviorNode> Outputs
        {
            get { return this.Where(x => x.Category == BehaviorCategory.Output); }
        }


        /// <summary>
        ///   Marking a BehaviorChain as "PartialOnly" means that no
        ///   Route will be generated and registered for this BehaviorChain.  
        ///   Set this property to true if you only want this BehaviorChain
        ///   to apply to partial requests.
        /// </summary>
        public bool IsPartialOnly { get; set; }

        /// <summary>
        ///   Models how the Route for this BehaviorChain will be generated
        /// </summary>
        public IRouteDefinition Route
        {
            get { return _route; }
            set
            {
                Trace(new RouteDetermined(value));
                _route = value;
            }
        }

        /// <summary>
        /// Collection of additional routes (aliases) that will be generated for this BehaviorChain.
        /// </summary>
        public IEnumerable<IRouteDefinition> AdditionalRoutes
        {
            get { return _additionalRoutes; }
        }

        /// <summary>
        /// Adds the specified route as an additional route for this BehaviorChain to respond to.
        /// </summary>
        /// <param name="route"></param>
        public void AddRouteAlias(IRouteDefinition route)
        {
            _additionalRoutes.Fill(route);
            Trace(new RouteAliasAdded(route));
        }

        /// <summary>
        ///   Categorizes this BehaviorChain for the IUrlRegistry and 
        ///   IEndpointService UrlFor(***, category) methods
        /// </summary>
        public UrlCategory UrlCategory { get; private set; }


        /// <summary>
        ///   Model of the authorization rules for this BehaviorChain
        /// </summary>
        public AuthorizationNode Authorization { get; private set; }

        public int Rank
        {
            get { return IsPartialOnly || Route == null ? 0 : Route.Rank; }
        }

        ObjectDef IContainerModel.ToObjectDef()
        {
            return buildObjectDef();
        }

        void IRegisterable.Register(Action<Type, ObjectDef> callback)
        {
            var objectDef = buildObjectDef();


            callback(typeof (IActionBehavior), objectDef);
            Authorization.As<IAuthorizationRegistration>().Register(Top.UniqueId, callback);
        }

        public string GetRoutePattern()
        {
            if (Route == null) return null;

            return Route.Pattern;
        }

        /// <summary>
        ///   Does this chain match by either UrlCategory or by Http method?
        /// </summary>
        /// <param name = "categoryOrHttpMethod"></param>
        /// <returns></returns>
        public bool MatchesCategoryOrHttpMethod(string categoryOrHttpMethod)
        {
            if (UrlCategory.Category.IsNotEmpty() &&
                UrlCategory.Category.Equals(categoryOrHttpMethod, StringComparison.OrdinalIgnoreCase)) return true;

            if (Route == null) return false;

            return Route.AllowedHttpMethods.Select(x => x.ToUpper()).Contains(categoryOrHttpMethod.ToUpper());
        }

        /// <summary>
        ///   Tests whether or not this chain has any output nodes
        /// </summary>
        /// <returns></returns>
        public bool HasOutput()
        {
            return (Top == null ? false : Top.HasAnyOutputBehavior()) ||
                   (_output.IsValueCreated && _output.Value.Writers.Any());
        }

        /// <summary>
        ///   Prepends the prefix to the route definition
        /// </summary>
        /// <param name = "prefix"></param>
        public void PrependToUrl(string prefix)
        {
            if (Route != null)
            {
                Route.Prepend(prefix);
            }
        }




        /// <summary>
        ///   What type of resource is rendered by this chain
        /// </summary>
        /// <returns></returns>
        public Type ResourceType()
        {
            if (_output.IsValueCreated)
            {
                return _output.Value.ResourceType;
            }

            return this.OfType<IMayHaveResourceType>().Reverse().FirstValue(x => x.ResourceType());
        }


        protected ObjectDef buildObjectDef()
        {
            return Top.As<IContainerModel>().ToObjectDef();
        }

        /// <summary>
        ///   The first ActionCall in this BehaviorChain.  Can be null.
        /// </summary>
        /// <returns></returns>
        public ActionCall FirstCall()
        {
            return Calls.FirstOrDefault();
        }

        /// <summary>
        ///   Returns the *last* ActionCall in this
        ///   BehaviorChain.  May be null.
        /// </summary>
        /// <returns></returns>
        public ActionCall LastCall()
        {
            return Calls.LastOrDefault();
        }

        /// <summary>
        ///   Returns the InputType of the very first
        /// </summary>
        /// <returns></returns>
        public Type InputType()
        {
            var calls = this.OfType<IMayHaveInputType>();
            if (calls.Any())
            {
                return calls.FirstValue(x => x.InputType());
            }

            // This is for chains with an actionless view
            if (HasOutput())
            {
                return ResourceType();
            }

            return null;
        }

        /// <summary>
        ///   Creates a new BehaviorChain for an action method
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "expression"></param>
        /// <returns></returns>
        public static BehaviorChain For<T>(Expression<Action<T>> expression)
        {
            var call = ActionCall.For(expression);
            var chain = new BehaviorChain();
            chain.AddToEnd(call);

            return chain;
        }

        /// <summary>
        ///   Checks to see if a Wrapper node of the requested behaviorType anywhere in the chain
        ///   regardless of position
        /// </summary>
        /// <param name = "behaviorType"></param>
        /// <returns></returns>
        public bool IsWrappedBy(Type behaviorType)
        {
            return this.Where(x => x is Wrapper).Cast<Wrapper>().Any(x => x.BehaviorType == behaviorType);
        }

        public override string ToString()
        {
            if (Route != null)
            {
                var description = Route.Pattern;
                if (Route.AllowedHttpMethods.Any())
                {
                    description += " (" + Route.AllowedHttpMethods.Join(", ") + ")";
                }

                return description;
            }

            if (Calls.Any())
            {
                return Calls.Select(x => x.Description).Join(", ");
            }

            return this.Select(x => x.Description).Join(" --> ");
        }

        public bool HasReaders()
        {
            return _input.IsValueCreated && _input.Value.Readers.Any();
        }

        /// <summary>
        ///   Allows you to explicitly force this BehaviorChain to the given
        ///   resource type.  This may be useful when the resource type cannot
        ///   be derived from the existing nodes.  Actionless view endpoints are
        ///   an example in the internals of this usage.
        /// </summary>
        /// <param name = "type"></param>
        public void ResourceType(Type type)
        {
            if (_output.IsValueCreated && _output.Value.ResourceType != type)
            {
                throw new InvalidOperationException("The ResourceType is already set for this chain");
            }

            if (!_output.IsValueCreated)
            {
                _output = new Lazy<OutputNode>(() => new OutputNode(type));
                if (Output.ResourceType != type)
                {
                    throw new ApplicationException("wouldn't really happen but I wanted to force the Lazy to evaluate");
                }
            }
        }

        public static BehaviorChain ForWriter(WriterNode node)
        {
            var chain = new BehaviorChain();
            chain.ResourceType(node.ResourceType);
            chain.Output.Writers.AddToEnd(node);

            return chain;
        }

        public bool HasResourceType()
        {
            var resourceType = ResourceType();
            return resourceType != null && !resourceType.CanBeCastTo<Task>() && resourceType != typeof (void);
        }

        /// <summary>
        /// General purpose tagging for behavior chains
        /// </summary>
        public readonly IList<string> Tags = new List<string>();

        public bool IsTagged(string tag)
        {
            return Tags.Any(x => x.EqualsIgnoreCase(tag));
        }
    }
}