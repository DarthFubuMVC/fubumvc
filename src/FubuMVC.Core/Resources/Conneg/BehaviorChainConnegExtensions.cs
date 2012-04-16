using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Resources.Media.Formatters;
using OutputNode = FubuMVC.Core.Resources.Conneg.New.OutputNode;

namespace FubuMVC.Core.Resources.Conneg
{
    public static class BehaviorChainConnegExtensions
    {
        // This should not do anything if there are conneg nodes
        public static void ApplyConneg(this BehaviorChain chain)
        {
            var inputType = chain.InputType();
            if (chain.InputNode() == null && inputType != null)
            {
                var inputNode = new InputNode(inputType);
                var action = chain.FirstCall();
                action.AddBefore(inputNode);
            }

            var actionOutputType = chain.ActionOutputType();
            if (chain.OutputNode() == null && actionOutputType != null && actionOutputType != typeof (void) && actionOutputType != typeof(HttpStatusCode))
            {
                var outputNode = new OutputNode(actionOutputType);
                var action = chain.Last(x => x is ActionCall);
                action.AddAfter(outputNode);
            }
        }

        public static void AlterConnegOutput(this BehaviorChain chain, Action<OutputNode> configure)
        {
            var node = chain.OutputNode();
            if (node != null)
            {
                configure(node);
            }
        }

        public static void AlterConnegInput(this BehaviorChain chain, Action<InputNode> configure)
        {
            var node = chain.InputNode();
            if (node != null)
            {
                configure(node);
            }
        }

        public static void UseFormatter<T>(this BehaviorChain chain) where T : IFormatter
        {
            chain.AlterConnegInput(node => node.AddFormatter<T>());
            chain.AlterConnegOutput(node => node.AddFormatter<T>());
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

            chain.AlterConnegInput(x => x.AddFormatter<JsonFormatter>());

            chain.AlterConnegOutput(x => x.JsonOnly());
        }

        public static bool IsAsymmetricJson(this BehaviorChain chain)
        {
            if (!chain.HasReaders() || !chain.HasOutput()) return false;

            if (chain.Output.Writers.Count() != 1) return false;
            if (!chain.Output.UsesFormatter<JsonFormatter>()) return false;

            if (chain.Input.Readers.Count() != 2) return false;
            if (!chain.Input.AllowHttpFormPosts) return false;

            return chain.Input.UsesFormatter<JsonFormatter>();
        }

        [Obsolete, MarkedForTermination]
        public static InputNode InputNode(this BehaviorChain chain)
        {
            return chain.FirstOrDefault(x => x is InputNode) as InputNode;
        }

        [Obsolete, MarkedForTermination]
        public static OutputNode OutputNode(this BehaviorChain chain)
        {
            return chain.FirstOrDefault(x => x is OutputNode) as OutputNode;
        }

        [Obsolete, MarkedForTermination]
        public static bool HasConnegOutput(this BehaviorChain chain)
        {
            return chain.OutputNode() != null;
        }

        public static void RemoveConneg(this BehaviorChain chain)
        {
            if (chain.HasReaders())
            {
                chain.Input.ClearAll();
            }

            if (chain.HasOutput())
            {
                chain.Output.ClearAll();
            }
        }

        public static void OutputJson(this BehaviorChain chain)
        {
            chain.ApplyConneg();
            chain.OutputNode().AddFormatter<JsonFormatter>();
        }

        public static void OutputXml(this BehaviorChain chain)
        {
            chain.ApplyConneg();
            chain.OutputNode().AddFormatter<XmlFormatter>();
        }


    }
}