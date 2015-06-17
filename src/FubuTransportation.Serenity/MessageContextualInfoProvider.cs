using System.Collections.Generic;
using System.Linq;
using FubuTransportation.Diagnostics;
using HtmlTags;
using Serenity;
using StoryTeller.Results;

namespace FubuTransportation.Serenity
{
    public class MessageContextualInfoProvider : Report
    {
        private readonly IMessagingSession _session;

        public MessageContextualInfoProvider(IMessagingSession session)
        {
            _session = session;
        }

        public string ToHtml()
        {
            return GenerateReports().ToTagList().ToString();
        }

        public string Title
        {
            get { return "Messaging Log"; }
        }

        public string ShortTitle
        {
            get { return "FubuTransportation"; }
        }

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