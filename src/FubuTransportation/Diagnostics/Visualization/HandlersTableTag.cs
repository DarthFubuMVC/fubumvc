using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuTransportation.Configuration;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Polling;
using FubuTransportation.Registration.Nodes;
using HtmlTags;

namespace FubuTransportation.Diagnostics.Visualization
{
    public class HandlersTableTag : TableTag
    {
        public HandlersTableTag(HandlerGraph handlers)
        {
            AddClass("table");
            AddClass("handlers");

            AddHeaderRow(row => {
                row.Header("Message");
                row.Header("Exception Policies");
                row.Header("Other");
                row.Header("Handlers");
            });

            handlers.Where(x => !FubuCore.TypeExtensions.Closes(x.InputType(), typeof(JobRequest<>))).OrderBy(x => x.InputType().Name)
                .Each(chain => AddBodyRow(row => addRow(row, chain)));
        }

        private void addRow(TableRowTag row, HandlerChain chain)
        {
            addMessageCell(row, chain);
            addExceptionCell(row, chain);
            addOthersCell(row, chain);

            row.Cell().Add("ul", ul => {
                chain.OfType<HandlerCall>().Each(call => {
                    ul.Add("li").Text(call.Description);
                });
            });
        }

        private static void addOthersCell(TableRowTag row, IEnumerable<BehaviorNode> chain)
        {
            var otherNodes =
                chain.Where(x => x.GetType() != typeof (ExceptionHandlerNode) && x.GetType() != typeof (HandlerCall));
            var otherCell = row.Cell();
            otherNodes.Each(node => {
                var description = Description.For(node);
                otherCell.Append(new DescriptionBodyTag(description));
            });
        }

        private static void addMessageCell(TableRowTag row, HandlerChain chain)
        {
            var messageCell = row.Cell().AddClass("message");
            messageCell.Add("h4").Text(chain.InputType().Name);
            messageCell.Add("p").Text(chain.InputType().Namespace);
        }

        private static void addExceptionCell(TableRowTag row, HandlerChain chain)
        {
            var exceptionCell = row.Cell();
            var node = chain.OfType<ExceptionHandlerNode>().FirstOrDefault();
            if (node != null)
            {
                var description = Description.For(node);
                var descriptionTag = new DescriptionBodyTag(description);
                exceptionCell.Append(descriptionTag);
            }
        }
    }
}