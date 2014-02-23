using System;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public static class BehaviorChainConnegExtensions
    {
        public static void AlterConnegOutput(this BehaviorChain chain, Action<OutputNode> configure)
        {
            var node = chain.Output;
            if (node != null)
            {
                configure(node);
            }
        }

        public static void AlterConnegInput(this BehaviorChain chain, Action<InputNode> configure)
        {
            var node = chain.Input;
            if (node != null)
            {
                configure(node);
            }
        }

        public static void UseJson(this BehaviorChain chain)
        {
            if (chain.InputType() != null) chain.Input.AddFormatter<JsonFormatter>();
            if (chain.HasResourceType()) chain.OutputJson();
        }

        public static void UseXml(this BehaviorChain chain)
        {
            if (chain.InputType() != null) chain.Input.AddFormatter<XmlFormatter>();
            if (chain.HasResourceType()) chain.OutputXml();
        }

        public static void OutputJson(this BehaviorChain chain)
        {
            chain.Output.AddFormatter<JsonFormatter>();
        }

        public static void OutputXml(this BehaviorChain chain)
        {
            chain.Output.AddFormatter<XmlFormatter>();
        }

        public static void MakeSymmetricJson(this BehaviorChain chain)
        {
            if (chain.InputType() != null)
            {
                chain.Input.ClearAll();
                chain.Input.AllowHttpFormPosts = false;
            }

            if (chain.ResourceType() != null)
            {
                chain.Output.ClearAll();
            }

            chain.UseJson();
        }

        public static void MakeAsymmetricJson(this BehaviorChain chain)
        {
            if (chain.InputType() != null)
            {
                chain.Input.ClearAll();
                chain.Input.AllowHttpFormPosts = true;
            }

            if (chain.ResourceType() != null)
            {
                chain.Output.ClearAll();
            }

            chain.UseJson();
        }

        public static bool WritesJson(this BehaviorChain chain)
        {
            return chain.Output.Writers.Any(x => x.Mimetypes.Contains(MimeType.Json.ToString()));
        }

        public static bool WritesXml(this BehaviorChain chain)
        {
            return chain.Output.Writers.Any(x => x.Mimetypes.Contains(MimeType.Xml.ToString()));
        }

        // TODO -- do something about matching on Mimetype maybe
        public static bool ReadsJson(this BehaviorChain chain)
        {
            return chain.Input.Readers.Any(x => x.Mimetypes.Contains(MimeType.Json.ToString()));
        }

        public static bool ReadsXml(this BehaviorChain chain)
        {
            return chain.Input.Readers.Any(x => x.Mimetypes.Contains(MimeType.Xml.ToString()));
        }

        // TODO -- change this to key off of mimetype only
        public static bool IsAsymmetricJson(this BehaviorChain chain)
        {
            if (chain.ResourceType() != null)
            {
                if (chain.Output.Writers.Count() != 1) return false;
                if (!chain.WritesJson()) return false;
            }

            if (chain.InputType() != null)
            {
                if (chain.Input.Readers.Count() != 2) return false;
                if (!chain.Input.AllowHttpFormPosts) return false;

                return chain.ReadsJson();
            }

            return true;
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
            }

            if (chain.HasResourceType())
            {
                chain.Output.ClearAll();
            }

            chain.UseJson();
            chain.UseXml();
        }
    }
}