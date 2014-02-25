using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Registration.Policies
{
    public class ConnegExpression
    {
        private readonly Policy _policy;

        public ConnegExpression(Policy policy)
        {
            _policy = policy;
        }

        /// <summary>
        /// The behavior chain will respond to either form posts (application/x-www-form-urlencoded) or json, and render
        /// as json
        /// </summary>
        public void MakeAsymmetricJson()
        {
            _policy.ModifyWith<AsymmetricJsonModification>();
        }

        /// <summary>
        /// The behavior chain will only respond to application/json or text/json, and renders json
        /// </summary>
        public void MakeSymmetricJson()
        {
            _policy.ModifyWith<SymmetricJsonModification>();
        }

        /// <summary>
        /// Add an additional Writer to chains matching this policy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddWriter<T>() where T : WriterNode, new()
        {
            _policy.ModifyWith<AddWriter<T>>();       
        }

        /// <summary>
        /// Add an additional Writer to chains matching this policy
        /// </summary>
        /// <param name="source"></param>
        /// <param name="description">Description of the writer for diagnostics</param>
        public void AddWriter(Func<BehaviorChain, WriterNode> source, string description)
        {
            _policy.ModifyWith(new AddWriter(source, description));
        }

        /// <summary>
        /// The resource (output) model will be rendered as text/html by calling
        /// ToString() on the resource model
        /// </summary>
        public void AddHtml()
        {
            _policy.ModifyWith<AddHtml>();
        }

        /// <summary>
        /// The chain will respond to form posts with a mimetype of 'application/x-www-form-urlencoded'
        /// </summary>
        public void AllowHttpFormPosts()
        {
            _policy.ModifyWith<AllowHttpFormPosts>();
        }

        /// <summary>
        /// The chain will accept Json by deserializing to the input type
        /// </summary>
        public void AcceptJson()
        {
            _policy.ModifyWith<AcceptJson>();
        }

        /// <summary>
        /// Add an additional writer to the chain
        /// </summary>
        /// <param name="writerType"></param>
        public void AddWriter(Type writerType)
        {
            _policy.ModifyBy(chain => chain.Output.AddWriter(writerType), configurationType:ConfigurationType.Conneg);
        }

        /// <summary>
        /// Removes any writers already registered against this chain
        /// </summary>
        public void ClearAllWriters()
        {
            _policy.ModifyBy(chain => chain.Output.ClearAll(), configurationType: ConfigurationType.Conneg);
        }

        /// <summary>
        /// Adds both json and xml formatters to the chain as well as accepting form posts with a mimetype of 'application/x-www-form-urlencoded'
        /// </summary>
        public void ApplyConneg()
        {
            _policy.ModifyBy(chain => chain.ApplyConneg(), configurationType: ConfigurationType.Conneg);
        }
    }

    [Title("Accept Json")]
    [ConfigurationType(ConfigurationType.Conneg)]
    public class AcceptJson : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            if (chain.InputType() == null) return;

            // TODO -- lets change this later
            chain.Input.Add(new JsonSerializer());
        }
    }

    [Title("Accept form posts as mimetype 'application/x-www-form-urlencoded'")]
    [ConfigurationType(ConfigurationType.Conneg)]
    public class AllowHttpFormPosts : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            if (chain.InputType() == null) return;

            // TODO -- let's change this later
            chain.Input.Add(typeof(ModelBindingReader<>));
        }
    }

    [Title("Write the resource type as text/html by calling ToString() on the resource")]
    [ConfigurationType(ConfigurationType.Conneg)]
    public class AddHtml : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.Output.AddHtml();
        }
    }

    [Title("Add Writer")]
    [ConfigurationType(ConfigurationType.Conneg)]
    public class AddWriter : IChainModification, DescribesItself
    {
        private readonly Func<BehaviorChain, WriterNode> _writerSource;
        private readonly string _description;

        public AddWriter(Func<BehaviorChain, WriterNode> writerSource, string description)
        {
            _writerSource = writerSource;
            _description = description;
        }

        public void Modify(BehaviorChain chain)
        {
            chain.Output.Writers.AddToEnd(_writerSource(chain));
        }

        public void Describe(Description description)
        {
            description.Properties["Writer"] = _description;
        }
    }

    public class AddWriter<T> : AddWriter where T : WriterNode, new()
    {
        public AddWriter() : base(chain => new T(), "Add " + typeof(T).Name)
        {
        }
    }

    [Title("Accepts Json or Http form posts, outputs Json only")]
    [ConfigurationType(ConfigurationType.Conneg)]
    public class AsymmetricJsonModification : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.MakeAsymmetricJson();
        }
    }

    [Title("Accepts and writes Json only")]
    [ConfigurationType(ConfigurationType.Conneg)]
    public class SymmetricJsonModification : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.MakeSymmetricJson();
        }
    }
}