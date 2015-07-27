using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;

namespace Serenity.ServiceBus
{
    public static class TestNodes
    {
        internal static readonly IList<Action<FubuRegistry>> Alterations = new List<Action<FubuRegistry>>(); 
        private static readonly IDictionary<string, ExternalNode> Nodes = new Dictionary<string, ExternalNode>();

        public static string[] Registries
        {
            get
            {
                return Assembly.GetExecutingAssembly()
                    .ExportedTypes.Where(x => x.IsConcreteTypeOf<FubuRegistry>())
                    .Select(x => x.Name)
                    .ToArray();
            }
        }

        public static ExternalNode AddNode(string name, ExternalNode node)
        {
            ExternalNode actualNode;
            if (Nodes.TryGetValue(name, out actualNode))
                return actualNode;

            node.Start();
            Nodes[name] = node;
            return node;
        }

        public static void ClearReceivedMessages()
        {
            Nodes.Values.Each(x => x.ClearReceivedMessages());
        }

        public static void Reset()
        {
            Nodes.Values.Each(x => x.SafeDispose());
            Nodes.Clear();
        }

        public static void OnNodeCreation(Action<FubuRegistry> alteration)
        {
            Alterations.Add(alteration);
        }
    }
}