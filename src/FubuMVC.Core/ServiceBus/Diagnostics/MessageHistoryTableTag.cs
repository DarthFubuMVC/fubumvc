using System.Collections.Generic;
using System.Linq;
using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public class MessageHistoryTableTag : TableTag
    {
        public MessageHistoryTableTag(MessageHistory history)
        {
            AddClass("table");

            Style("width", "inherit");
            Style("margin-top", "50px");

            AddHeaderRow(tr => {
                tr.Header(history.Description).Attr("colspan", "5").Style("text-align", "left");
            });

            AddHeaderRow(tr => {
                tr.Header("Node").Style("text-align", "left");
                tr.Header("Timestamp").Style("text-align", "left");
                tr.Header("Message").Style("text-align", "left").Style("width", "200px");
                tr.Header("Header = ").Style("text-align", "right");
                tr.Header("Value").Style("text-align", "left");
            });

           

            history.Records().Each(rec => {
                var headers = rec.Headers.IsNotEmpty()  ? rec.Headers.Split(';').ToArray() : new string[0];
                

                AddBodyRow(tr => {
                    tr.Cell(rec.Node).Attr("valign", "top").Style("padding-right", "30px");
                    tr.Cell(rec.Timestamp.ToLongTimeString()).Style("padding-right", "30px").Attr("valign", "top").Attr("nowrap", "true");
                    tr.Cell(rec.Message).Attr("valign", "top").Style("padding-right", "30px");
                    
                    if (headers.Any())
                    {
                        var count = headers.Count().ToString();
                        tr.Children.Each(x => x.Attr("rowspan", count));

                        string headerValue = headers.First();
                        writeHeaderValue(headerValue, tr);
                    }
                });

                for (int i = 1; i < headers.Count(); i++)
                {
                    AddBodyRow(tr => writeHeaderValue(headers[i], tr));
                }

                if (rec.ExceptionText.IsNotEmpty())
                {
                    AddBodyRow(tr => {
                        var cell = tr.Cell();
                        cell.Attr("colspan", "5");


                        cell.Add("pre").Text(rec.ExceptionText).Style("background-color", "#FFFFAA");
                    });
                }
            });
        }

        private static void writeHeaderValue(string headerValue, TableRowTag tr)
        {
            var parts = headerValue.Split('=');
            tr.Cell(parts.First() + " = ").Attr("valign", "top").Style("text-align", "right").Attr("nowrap", "true");
            tr.Cell(parts.Last()).Attr("valign", "top").Attr("nowrap", "true");
        }
    }
}