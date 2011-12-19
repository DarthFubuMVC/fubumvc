using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg
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
            if (chain.ConnegOutputNode() == null && actionOutputType != null && actionOutputType != typeof (void) && actionOutputType != typeof(HttpStatusCode))
            {
                var outputNode = new ConnegOutputNode(actionOutputType);
                var action = chain.Last(x => x is ActionCall);
                action.AddAfter(outputNode);
            }
        }

        public static void AlterConnegOutput(this BehaviorChain chain, Action<ConnegOutputNode> configure)
        {
            var node = chain.ConnegOutputNode();
            if (node != null)
            {
                configure(node);
            }
        }

        public static void AlterConnegInput(this BehaviorChain chain, Action<ConnegInputNode> configure)
        {
            var node = chain.ConnegInputNode();
            if (node != null)
            {
                configure(node);
            }
        }

        public static void UseFormatter<T>(this BehaviorChain chain) where T : IFormatter
        {
            chain.AlterConnegInput(node => node.UseFormatter<T>());
            chain.AlterConnegOutput(node => node.UseFormatter<T>());
        }

        public static void MakeSymmetricJson(this BehaviorChain chain)
        {
            chain.RemoveConneg();
            chain.ApplyConneg();

            chain.AlterConnegInput(x => x.JsonOnly());
            chain.AlterConnegOutput(x => x.JsonOnly());
        }

        public static void MakeAsymmetricJson(this BehaviorChain chain)
        {
            chain.RemoveConneg();
            chain.ApplyConneg();

            chain.AlterConnegInput(x => x.UseFormatter<JsonFormatter>());

            chain.AlterConnegOutput(x => x.JsonOnly());
        }

        public static bool IsAsymmetricJson(this BehaviorChain chain)
        {
            if (chain.ConnegInputNode() == null && chain.ConnegOutputNode() == null) return false;

            if (chain.ActionOutputType() != null)
            {
                var output = chain.ConnegOutputNode();
                if (output.SelectedFormatterTypes.Count() != 1) return false;

                if (output.SelectedFormatterTypes.Single() != typeof(JsonFormatter)) return false;
            }

            if (chain.InputType() != null)
            {
                var input = chain.ConnegInputNode();

                if (!input.AllowHttpFormPosts)
                {
                    return false;
                }
                
                if (input.SelectedFormatterTypes.Count() != 1) return false;

                if (input.SelectedFormatterTypes.Single() != typeof(JsonFormatter)) return false;
            }

            return true;
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