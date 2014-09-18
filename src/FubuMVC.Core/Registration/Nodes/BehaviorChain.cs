using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;

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
        private readonly Lazy<InputNode> _input;
        private Lazy<OutputNode> _output;
        private readonly Lazy<AuthorizationNode> _authorization = new Lazy<AuthorizationNode>(() => new AuthorizationNode()); 

        public BehaviorChain()
        {
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

        public IOutputNode Output
        {
            get { return _output.Value; }
        }

        public IInputNode Input
        {
            get { return _input.Value; }
        }


        internal protected virtual void InsertNodes(ConnegSettings settings)
        {
            if (HasResourceType() && !ResourceType().CanBeCastTo<FubuContinuation>())
            {
                var outputNode = _output.Value;
                outputNode.UseSettings(settings);

                AddToEnd(outputNode);
            }

            if (_authorization.IsValueCreated && Authorization.HasRules())
            {
                Prepend(_authorization.Value);
            }

            if (InputType() != null)
            {
                _input.Value.UseSettings(settings);
                Prepend(_input.Value);
            }

            this.OfType<IModifiesChain>().ToArray().Each(x => x.Modify(this));
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
        ///   Model of the authorization rules for this BehaviorChain
        /// </summary>
        public IAuthorizationNode Authorization {
            get
            {
                return _authorization.Value;
            }
        }

        /// <summary>
        ///   Categorizes this BehaviorChain for the IUrlRegistry and 
        ///   IEndpointService UrlFor(***, category) methods
        /// </summary>
        public UrlCategory UrlCategory { get; private set; }


        public virtual string Category
        {
            get { return UrlCategory == null ? null : UrlCategory.Category; }
        }

        ObjectDef IContainerModel.ToObjectDef()
        {
            return buildObjectDef();
        }

        void IRegisterable.Register(Action<Type, ObjectDef> callback)
        {
            if (Top == null)
            {
                Console.WriteLine("Some how or another me, a fully formed BehaviorChain, has no BehaviorNode's, so I'm a just gonna punt on registering services");
                return;
            }

            var objectDef = buildObjectDef();


            callback(typeof (IActionBehavior), objectDef);
        }

        /// <summary>
        ///   Tests whether or not this chain has any output nodes
        /// </summary>
        /// <returns></returns>
        public bool HasOutput()
        {
            return (Top == null ? false : Top.HasAnyOutputBehavior()) ||
                   (_output.IsValueCreated && _output.Value.Media().Any());
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
            if (Calls.Any())
            {
                return Calls.Select(x => x.Description).Join(", ");
            }

            return this.Select(x => x.Description).Join(" --> ");
        }

        public bool HasReaders()
        {
            return _input.IsValueCreated && _input.Value.Readers().Any();
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
            _output = new Lazy<OutputNode>(() => new OutputNode(type));
            if (_output.Value.ResourceType != type)
            {
                throw new ApplicationException("wouldn't really happen but I wanted to force the Lazy to evaluate");
            }
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

        /// <summary>
        ///   Does this chain match by either UrlCategory or by Http method?
        /// </summary>
        /// <param name = "categoryOrHttpMethod"></param>
        /// <returns></returns>
        public virtual bool MatchesCategoryOrHttpMethod(string categoryOrHttpMethod)
        {
            if (categoryOrHttpMethod == Categories.DEFAULT) return true;

            if (categoryOrHttpMethod.EqualsIgnoreCase(Category)) return true;

            return IsTagged(categoryOrHttpMethod);
        }

        public static BehaviorChain ForResource(Type resourceType)
        {
            var chain = new BehaviorChain();
            chain._output = new Lazy<OutputNode>(() => new OutputNode(resourceType));

            return chain;
        }

        public virtual string Title()
        {
            var title = "Partial: ";


            if (Calls.Any())
            {
                title += Calls.Select(x => x.Description).Join(", ");
                return title;
            }

            if (Tags.Contains("ActionlessView"))
            {
                var views = Output.Media().Select(x => x.Writer).OfType<IViewWriter>().Select(x => Description.For(x.View).Title);
                return "View(s): " + views.Join("");
            }

            if (HasOutput() && Output.Media().Any())
            {
                return Output.Media().Select(x => Description.For(x.Writer).Title).Join(", ");
            }

            if (InputType() != null)
            {
                return "Handler for " + InputType().FullName;
            }

            return "BehaviorChain " + UniqueId;
        }

        public override int GetHashCode()
        {
            return Title().GetHashCode();
        }
    }
}