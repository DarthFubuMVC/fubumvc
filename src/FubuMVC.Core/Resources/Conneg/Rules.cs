using System.Collections.Generic;
using FubuCore;
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

    public class DefaultReadersAndWriters : ConnegRule
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
    }

    public class AjaxContinuations : ConnegRule
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
    }


   
    public class StringOutput : ConnegRule
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
    }

    public class HtmlTagsRule : ConnegRule
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
    }

    public class SymmetricJson : ConnegRule
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
    }

    public class AsymmetricJson : ConnegRule
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.AnyActionHasAttribute<AsymmetricJsonAttribute>())
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
    }

}