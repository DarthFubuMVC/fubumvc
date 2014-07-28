using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ConnegRules : Chain<ConnegRule, ConnegRules>
    {
        
    }

    public abstract class ConnegRule : Node<ConnegRule, ConnegRules>
    {
        public void ApplyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (applyInputs(node, chain, settings) == DoNext.Continue && Next != null)
            {
                Next.ApplyInputs(node, chain, settings);
            }
        }

        protected virtual DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            return DoNext.Continue;
        }

        public void ApplyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (applyOutputs(node, chain, settings) == DoNext.Continue && Next != null)
            {
                Next.ApplyOutputs(node, chain, settings);
            }
        }

        protected virtual DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            return DoNext.Continue;
        }
    }

    public class CustomReadersAndWriters : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            var graph = settings.Graph ?? new ConnegGraph();
            graph.ReaderTypesFor(node.InputType()).Each(type => node.Add(Activator.CreateInstance(type).As<IReader>()));

            return DoNext.Continue;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            var graph = settings.Graph ?? new ConnegGraph();
            graph.WriterTypesFor(node.ResourceType).Each(type => node.Add(Activator.CreateInstance(type)));

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "Applies any custom readers and writers found in the loaded assemblies to a chain based on the Input and Resource types";
        }
    }

    public class DefaultReadersAndWriters : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            node.Add(typeof(ModelBindingReader<>));
            node.Add(settings.FormatterFor(MimeType.Json));
            node.Add(settings.FormatterFor(MimeType.Xml));

            return DoNext.Continue;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            node.Add(settings.FormatterFor(MimeType.Json));
            node.Add(settings.FormatterFor(MimeType.Xml));

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "Accept Json, Xml, and HTTP form posts, writes Json or Xml with Json being the default in both cases";
        }
    }

    public class AjaxContinuations : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (!chain.ResourceType().CanBeCastTo<AjaxContinuation>()) return DoNext.Continue;

            node.Add(typeof(ModelBindingReader<>));
            node.Add(settings.FormatterFor(MimeType.Json));

            return DoNext.Stop;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (!chain.ResourceType().CanBeCastTo<AjaxContinuation>()) return DoNext.Continue;
       
            node.Add(typeof(AjaxContinuationWriter<>));

            return DoNext.Stop;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "If any action returns AjaxContinuation or a subtype, accept Json or HTTP form posts and only output Json with the AjaxContinuationWriter";
        }
    }


   
    public class StringOutput : ConnegRule, DescribesItself
    {
        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.ResourceType() == typeof (string))
            {
                node.Add(new StringWriter());

                return DoNext.Stop;
            }

            return base.applyOutputs(node, chain, settings);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "If an action returns a .Net string, the chain will only render text/plain";
        }
    }

    public class HtmlTagsRule : ConnegRule, DescribesItself
    {
        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.ResourceType().CanBeCastTo<HtmlTag>() || chain.ResourceType().CanBeCastTo<HtmlDocument>())
            {
                node.Add(typeof(HtmlStringWriter<>));
                return DoNext.Stop;
            }

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "If an action returns a model type that inherits from either HtmlTag or HtmlDocument, the chain will only output HTML";
        }
    }


    public class SymmetricJson : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.AnyActionHasAttribute<SymmetricJsonAttribute>())
            {
                node.Add(settings.FormatterFor(MimeType.Json));

                return DoNext.Stop;
            }

            return DoNext.Continue;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.AnyActionHasAttribute<SymmetricJsonAttribute>() || chain.ResourceType().CanBeCastTo<IDictionary<string, object>>())
            {
                node.Add(settings.FormatterFor(MimeType.Json));

                return DoNext.Stop;
            }

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "If an action is decorated with the [SymmetricJson] attribute, the chain will only read and write JSON with the registered Formatter for Json";
        }
    }

    public class AsymmetricJson : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.AnyActionHasAttribute<AsymmetricJsonAttribute>() || chain.ResourceType().CanBeCastTo<IDictionary<string, object>>())
            {
                node.Add(typeof(ModelBindingReader<>));
                node.Add(settings.FormatterFor(MimeType.Json));

                return DoNext.Stop;
            }
            
            return DoNext.Continue;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.AnyActionHasAttribute<AsymmetricJsonAttribute>() || chain.ResourceType().CanBeCastTo<IDictionary<string, object>>())
            {
                node.Add(settings.FormatterFor(MimeType.Json));

                return DoNext.Stop;
            }

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "If an action is decorated with the [AsymmetricJson] attribute, the chain will read JSON or use model binding for HTTP form posts, but always return Json";
        }
    }

}