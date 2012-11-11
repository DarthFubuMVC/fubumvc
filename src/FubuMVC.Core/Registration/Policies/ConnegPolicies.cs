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

        public void MakeAsymmetricJson()
        {
            _policy.ModifyWith<AsymmetricJsonModification>();
        }

        public void MakeSymmetricJson()
        {
            _policy.ModifyWith<SymmetricJsonModification>();
        }

        public void AddWriter<T>() where T : WriterNode, new()
        {
            _policy.ModifyWith<AddWriter<T>>();       
        }

        public void AddWriter(Func<BehaviorChain, WriterNode> source)
        {
            _policy.ModifyWith(new AddWriter(source));
        }

        public void AddHtml()
        {
            _policy.ModifyWith<AddHtml>();
        }

        public void AllowHttpFormPosts()
        {
            _policy.ModifyWith<AllowHttpFormPosts>();
        }

        public void AcceptJson()
        {
            _policy.ModifyWith<AcceptJson>();
        }

        public void AddWriter(Type writerType)
        {
            _policy.ModifyBy(chain => chain.Output.AddWriter(writerType));
        }

        public void ClearAllWriters()
        {
            _policy.ModifyBy(chain => chain.Output.ClearAll());
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