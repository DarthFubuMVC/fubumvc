using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class SerializersTag : TableTag
    {
        public SerializersTag(IEnumerable<IMessageSerializer> serializers)
        {
            AddClass("table");

            AddHeaderRow(row => {
                                    row.Header("Serializer");
                                    row.Header("Description");
                                    row.Header("Content Type");
            });

            serializers.Each(x => {
                                      var description = Description.For(x);
                                      AddBodyRow(row =>
                                      {
                                          row.Cell(description.Title).AddClass("title");
                                          row.Cell().AddClass("description").Text(description.ShortDescription);
                                          row.Cell(x.ContentType);
                                      });
            });
        }
    }
}