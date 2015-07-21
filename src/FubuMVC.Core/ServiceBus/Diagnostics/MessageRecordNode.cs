using System;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public abstract class MessageRecordNode
    {
        public DateTime Timestamp = DateTime.Now; // Yes, use local time because it's meant to be read by humans

        public abstract HtmlTag ToLeafTag();
    }
}