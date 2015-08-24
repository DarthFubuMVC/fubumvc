using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Conneg
{
    public class CustomReadersAndWriters : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            var graph = settings.Graph ?? new ConnegGraph();
            graph.ReaderTypesFor(node.InputType()).Each(type => node.Add(Activator.CreateInstance((Type) type).As<IReader>()));

            return DoNext.Continue;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            var graph = settings.Graph ?? new ConnegGraph();
            graph.WriterTypesFor(node.ResourceType).Each(type => node.Add((IMediaWriter) Activator.CreateInstance(type)));

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "Applies any custom readers and writers found in the loaded assemblies to a chain based on the Input and Resource types";
        }
    }
}