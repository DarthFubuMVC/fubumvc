using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Subscriptions;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class SubscriptionsTableTag : TableTag
    {
        public SubscriptionsTableTag(IEnumerable<Subscription> subscriptions)
        {
            AddClass("table");
            AddClass("subscriptions");

            AddHeaderRow(row => {
                row.Header("Receiver");
                row.Header("Message Type");
            });

            subscriptions.OrderBy(x => x.MessageType).ThenBy(x => x.Receiver.ToString()).Each(addSubscription);
        }

        private void addSubscription(Subscription s)
        {
            AddBodyRow(row => {
                row.Cell(s.Receiver.ToString());
                row.Cell(s.MessageType);
            });
        }
    }

    public class SubscriptionStorageTableTag : TableTag
    {
        public SubscriptionStorageTableTag(IEnumerable<Subscription> subscriptions)
        {
            AddClass("table");
            AddClass("subscriptions");

            AddHeaderRow(row => {
                row.Header("Id");
                row.Header("Source");
                row.Header("Receiver");
                row.Header("Message Type");
                row.Header("Node Name");
                row.Header("Role");
            });

            subscriptions.OrderBy(x => x.MessageType).ThenBy(x => x.Receiver.ToString()).Each(addSubscription);
        }

        private void addSubscription(Subscription s)
        {
            AddBodyRow(row => {
                row.Cell(s.Id.ToString());
                row.Cell(s.Source.ToString());
                row.Cell(s.Receiver.ToString());
                row.Cell(s.MessageType);
                row.Cell(s.NodeName);
                row.Cell(s.Role.ToString());
            });
        }
    }

    public class TransportNodeTableTag : TableTag
    {
        public TransportNodeTableTag(IEnumerable<TransportNode> nodes)
        {
            AddClass("table");
            AddClass("nodes");

            AddHeaderRow(row => {
                row.Header("Id");
                row.Header("Machine");
                row.Header("Node Name");
                row.Header("Addresses");
            });

            nodes.OrderBy(x => x.MachineName).ThenBy(x => x.NodeName).Each(addNode);
        }

        private void addNode(TransportNode transportNode)
        {
            AddBodyRow(row => {
                row.Cell(transportNode.Id.ToString());
                row.Cell(transportNode.MachineName);
                row.Cell(transportNode.NodeName);
                row.Cell(transportNode.Addresses.Select(x => x.ToString()).Join(", "));
            });
        }
    }
}