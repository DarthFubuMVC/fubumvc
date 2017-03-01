using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Configuration;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class ChannelsTableTag : TableTag
    {
        public ChannelsTableTag(ChannelGraph graph)
        {
            AddClass("table");
            AddClass("channels");

            AddHeaderRow(row => {
                row.Header("Description");
                row.Header("Incoming Scheduler");
                row.Header("Routing Rules");
                row.Header("Serialization Default");
                row.Header("Accepted Content Types");
                row.Header("Modifiers");
            });

            graph.OrderBy(x => x.Key).Each(channel => {
                AddBodyRow(row => addRow(row, channel, graph));
            });
        }

        private void addRow(TableRowTag row, ChannelNode channel, ChannelGraph graph)
        {
            addDescriptionCell(row, channel);

            addSchedulers(row, channel);

            addRoutingRules(row, channel);

            addSerialization(row, channel);

            addAcceptedContentTypes(row, channel, graph);

            addModifiers(row, channel);
        }

        private void addModifiers(TableRowTag row, ChannelNode channel)
        {
            var cell = row.Cell().AddClass("modifiers");
            if (channel.Modifiers.Any())
            {
                cell.Add("ul", ul => { channel.Modifiers.Each(x => ul.Add("li").Text(x.ToString())); });
            }
            else
            {
                cell.Text("None");
            }
        }

        private void addSerialization(TableRowTag row, ChannelNode channel)
        {
            var cell = row.Cell().AddClass("serialization");
            if (channel.DefaultContentType.IsNotEmpty())
            {
                cell.Text(channel.DefaultContentType);
            }
            else if (channel.DefaultSerializer != null)
            {
                cell.Text(channel.DefaultSerializer.ToString());
            }
            else
            {
                cell.Text("None");
            }
        }

        private void addAcceptedContentTypes(TableRowTag row, ChannelNode channel, ChannelGraph graph)
        {
            var cell = row.Cell();
            var acceptedContentTypes = graph.GetAcceptedContentTypesForChannel(channel.Uri);
            if (acceptedContentTypes.Any())
            {
                cell.AppendHtml(acceptedContentTypes.Join("<br/>"));
            }
            else
            {
                cell.Text("None");
            }

        }

        private static void addRoutingRules(TableRowTag row, ChannelNode channel)
        {
            var cell = row.Cell().AddClass("routing-rules");
            if (channel.Rules.Any())
            {
                cell.Add("ul", ul => { channel.Rules.Each(x => ul.Add("li").Text(x.Describe())); });
            }
            else
            {
                cell.Text("None");
            }
        }

        private static void addSchedulers(TableRowTag row, ChannelNode channel)
        {
            var cell = row.Cell();
            if (channel.Incoming)
            {
                var description = Description.For(channel.Scheduler);
                cell.Append(new DescriptionBodyTag(description));
            }
            else
            {
                cell.Text("None");
            }
        }

        private static void addDescriptionCell(TableRowTag row, ChannelNode channel)
        {
            var cell = row.Cell();
            cell.Add("h5").Text(channel.Key);
            cell.Add("div/i").Text(channel.Uri.ToString());
            if (channel.DefaultContentType != null)
                cell.Add("div").Text("Default Content Type: " + channel.DefaultContentType);
        }
    }
}
