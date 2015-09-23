using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Diagnostics;
using HtmlTags;
using StoryTeller.Results;

namespace Serenity.ServiceBus
{
    public class MessageContextualInfoProvider : Report
    {
        private readonly IMessagingSession _session;

        public MessageContextualInfoProvider(IMessagingSession session)
        {
            _session = session;

            Title = "Messaging Log";
            ShortTitle = "ServiceBus";
        }


        public string ToHtml()
        {
            return GenerateReports().ToTagList().ToString();
        }

        public string Title { get; set; }

        public string ShortTitle { get; set; }

        public int Count
        {
            get { return _session.TopLevelMessages().Count(); }
        }

        public IEnumerable<HtmlTag> GenerateReports()
        {
            yield return new HtmlTag("h3").Text("Message History");

            yield return new HtmlTag("ol", x =>
            {
                foreach (MessageHistory topLevelMessage in _session.TopLevelMessages().ToList())
                {
                    x.Append(topLevelMessage.ToLeafTag());
                }
            });

            foreach (MessageHistory history in _session.AllMessages().ToList())
            {
                yield return new MessageHistoryTableTag(history);
            }
        }
    }

}