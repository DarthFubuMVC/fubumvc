using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class TransportsTag : TableTag
    {
        public TransportsTag(IEnumerable<ITransport> transports)
        {
            AddClass("table");
            AddClass("transports");

            transports.Each(x => {
                var description = Description.For(x);

                AddBodyRow(row => {
                    row.Cell(description.Title).AddClass("title");
                    row.Cell().AddClass("description").Text(description.ShortDescription);
                });
            });
        }
    }
}