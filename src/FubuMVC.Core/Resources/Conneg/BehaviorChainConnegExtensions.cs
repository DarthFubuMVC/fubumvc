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
            // TODO -- this needs to change
            if (chain.InputType() != null) chain.Input.Add(new JsonSerializer());
            if (chain.HasResourceType()) chain.OutputJson();
        }

        public static void UseXml(this BehaviorChain chain)
        {
            if (chain.InputType() != null) chain.Input.Add(new XmlFormatter());
            if (chain.HasResourceType()) chain.OutputXml();
        }

        public static void OutputJson(this BehaviorChain chain)
        {
            // TODO -- this has to change
            chain.Output.Add(new JsonSerializer());

        }

        public static void OutputXml(this BehaviorChain chain)
        {
            // TODO -- this has to change
            chain.Output.Add(new XmlFormatter());
        }

        public static void MakeSymmetricJson(this BehaviorChain chain)
        {
            if (chain.InputType() != null)
            {
                chain.Input.ClearAll();
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
                chain.Input.Add(typeof(ModelBindingReader<>));
            }

            if (chain.ResourceType() != null)
            {
                chain.Output.ClearAll();
            }

            chain.UseJson();
        }

        public static bool WritesJson(this BehaviorChain chain)
        {
            return chain.Output.MimeTypes().Contains(MimeType.Json.ToString());
        }

        public static bool WritesXml(this BehaviorChain chain)
        {
            return chain.Output.MimeTypes().Contains(MimeType.Xml.ToString());
        }

        // TODO -- do something about matching on Mimetype maybe
        public static bool ReadsJson(this BehaviorChain chain)
        {
            return chain.Input.CanRead(MimeType.Json);
        }

        public static bool ReadsXml(this BehaviorChain chain)
        {
            return chain.Input.CanRead(MimeType.Html);
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
                chain.Input.Add(typeof(ModelBindingReader<>));
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