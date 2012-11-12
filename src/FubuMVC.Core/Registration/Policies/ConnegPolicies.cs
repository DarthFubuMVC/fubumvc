using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
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
        public void AddWriter(Func<BehaviorChain, WriterNode> source)
        {
            _policy.ModifyWith(new AddWriter(source));
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
            _policy.ModifyBy(chain => chain.Output.AddWriter(writerType));
        }

        /// <summary>
        /// Removes any writers already registered against this chain
        /// </summary>
        public void ClearAllWriters()
        {
            _policy.ModifyBy(chain => chain.Output.ClearAll());
        }

        /// <summary>
        /// Adds both json and xml formatters to the chain as well as accepting form posts with a mimetype of 'application/x-www-form-urlencoded'
        /// </summary>
        public void ApplyConneg()
        {
            _policy.ModifyBy(chain => chain.ApplyConneg());
        }
    }

    public class AcceptJson : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            if (chain.InputType() == null) return;

            chain.Input.AddFormatter<JsonFormatter>();
        }
    }

    public class AllowHttpFormPosts : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            if (chain.InputType() == null) return;

            chain.Input.AllowHttpFormPosts = true;
        }
    }

    public class AddHtml : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.Output.AddHtml();
        }
    }

    public class AddWriter : IChainModification
    {
        private readonly Func<BehaviorChain, WriterNode> _writerSource;

        public AddWriter(Func<BehaviorChain, WriterNode> writerSource)
        {
            _writerSource = writerSource;
        }

        public void Modify(BehaviorChain chain)
        {
            chain.Output.Writers.AddToEnd(_writerSource(chain));
        }
    }

    public class AddWriter<T> : AddWriter where T : WriterNode, new()
    {
        public AddWriter() : base(chain => new T())
        {
        }
    }

    public class AsymmetricJsonModification : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.MakeAsymmetricJson();
        }
    }

    public class SymmetricJsonModification : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.MakeSymmetricJson();
        }
    }
}