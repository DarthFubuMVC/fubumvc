using System;
using System.Linq;
using System.Net;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime.Formatters;
using OutputNode = FubuMVC.Core.Resources.Conneg.New.OutputNode;

namespace FubuMVC.Core.Resources.Conneg
{
    public static class BehaviorChainConnegExtensions
    {
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
            chain.Input.AddFormatter<T>();
            chain.Output.AddFormatter<T>();
        }

        public static void MakeSymmetricJson(this BehaviorChain chain)
        {
            if (chain.InputType() != null)
            {
                chain.Input.ClearAll();
                chain.Input.AllowHttpFormPosts = false;
                chain.Input.AddFormatter<JsonFormatter>();
            }

            if (chain.ResourceType() != null)
            {
                chain.Output.ClearAll();
                chain.Output.AddFormatter<JsonFormatter>();
            }
        }

        public static void MakeAsymmetricJson(this BehaviorChain chain)
        {
            if (chain.InputType() != null)
            {
                chain.Input.ClearAll();
                chain.Input.AllowHttpFormPosts = true;
                chain.Input.AddFormatter<JsonFormatter>();
            }

            if (chain.ResourceType() != null)
            {
                chain.Output.ClearAll();
                chain.Output.AddFormatter<JsonFormatter>();
            }
        }

        public static bool IsAsymmetricJson(this BehaviorChain chain)
        {

            if (chain.ResourceType() != null)
            {
                if (chain.Output.Writers.Count() != 1) return false;
                if (!chain.Output.UsesFormatter<JsonFormatter>()) return false;
            }

            if (chain.InputType() != null)
            {
                if (chain.Input.Readers.Count() != 2) return false;
                if (!chain.Input.AllowHttpFormPosts) return false;

                return chain.Input.UsesFormatter<JsonFormatter>();
            }

            return true;
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
            chain.Output.AddFormatter<JsonFormatter>();
        }

        public static void OutputXml(this BehaviorChain chain)
        {
            chain.Output.AddFormatter<XmlFormatter>();
        }

        /// <summary>
        /// Sets up very basic content negotiation for an endpoint.
        /// Accepts http form posts, xml, and json
        /// Returns xml or json
        /// </summary>
        /// <param name="chain"></param>
        public static void ApplyConneg(this BehaviorChain chain)
        {
            chain.RemoveConneg();

            if (chain.InputType() != null)
            {
                chain.Input.ClearAll();
                chain.Input.AllowHttpFormPosts = true;
                chain.Input.AddFormatter<JsonFormatter>();
                chain.Input.AddFormatter<XmlFormatter>();
            }

            if (chain.ResourceType() != null)
            {
                chain.Output.ClearAll();
                chain.Output.AddFormatter<JsonFormatter>();
                chain.Output.AddFormatter<XmlFormatter>();
            }
        }
    }
}