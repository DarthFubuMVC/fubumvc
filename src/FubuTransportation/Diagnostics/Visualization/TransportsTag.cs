using System.Collections.Generic;
using System.Security.Cryptography;
using FubuCore.Descriptions;
using FubuTransportation.Runtime;
using HtmlTags;

namespace FubuTransportation.Diagnostics.Visualization
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