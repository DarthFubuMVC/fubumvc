using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Rest.Media.Formatters;

namespace FubuMVC.Core.Rest.Conneg
{
    public static class BehaviorChainConnegExtensions
    {
        // This should not do anything if there are conneg nodes
        public static void ApplyConneg(this BehaviorChain chain)
        {
            var inputType = chain.InputType();
            if (chain.ConnegInputNode() == null && inputType != null)
            {
                var inputNode = new ConnegInputNode(inputType);
                var action = chain.FirstCall();
                action.AddBefore(inputNode);
            }

            var actionOutputType = chain.ActionOutputType();
            if (chain.ConnegOutputNode() == null && actionOutputType != null && actionOutputType != typeof (void))
            {
                var outputNode = new ConnegOutputNode(actionOutputType);
                var action = chain.Last(x => x is ActionCall);
                action.AddAfter(outputNode);
            }
        }

        public static void MakeSymmetricJson(this BehaviorChain chain)
        {
            chain.RemoveConneg();
            chain.ApplyConneg();


            var connegInputNode = chain.ConnegInputNode();
            if (connegInputNode != null) connegInputNode.JsonOnly();

            var connegOutputNode = chain.ConnegOutputNode();
            if (connegOutputNode != null) connegOutputNode.JsonOnly();
        }

        public static void MakeAsymmetricJson(this BehaviorChain chain)
        {
            chain.RemoveConneg();
            chain.ApplyConneg();

            chain.ConnegInputNode().UseFormatter<JsonFormatter>();
            chain.ConnegOutputNode().JsonOnly();
        }

        public static ConnegInputNode ConnegInputNode(this BehaviorChain chain)
        {
            return chain.FirstOrDefault(x => x is ConnegInputNode) as ConnegInputNode;
        }

        public static ConnegOutputNode ConnegOutputNode(this BehaviorChain chain)
        {
            return chain.FirstOrDefault(x => x is ConnegOutputNode) as ConnegOutputNode;
        }

        public static bool HasConnegOutput(this BehaviorChain chain)
        {
            return chain.ConnegOutputNode() != null;
        }

        public static void RemoveConneg(this BehaviorChain chain)
        {
            chain.OfType<ConnegNode>().ToList().Each(x => x.Remove());
        }

        public static void OutputJson(this BehaviorChain chain)
        {
            chain.ApplyConneg();
            chain.ConnegOutputNode().UseFormatter<JsonFormatter>();
        }

        public static void OutputXml(this BehaviorChain chain)
        {
            chain.ApplyConneg();
            chain.ConnegOutputNode().UseFormatter<XmlFormatter>();
        }
    }
}