using System.Collections.Generic;
using System.Linq;
using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public class MessageLog : MessageRecordNode
    {
        private readonly IList<MessageLog> _children = new List<MessageLog>();
        private readonly IList<MessageRecord> _records = new List<MessageRecord>(); 

        public string Id { get; set; }
        public string Type { get; set; }

        public string Description
        {
            get { return "Message {0} ({1})".ToFormat(Id, Type); }
        }

        public IEnumerable<MessageRecord> Records()
        {
            return _records.OrderBy(x => x.Timestamp);
        }

        public void AddChild(MessageLog child)
        {
            _children.Fill(child);
            child.Parent = this;
        }

        public MessageLog Parent { get; set; }


        protected bool Equals(MessageLog other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MessageLog) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private IEnumerable<MessageRecordNode> allRecordNodes()
        {
            foreach (var child in _children)
            {
                yield return child;
            }

            foreach (var record in _records)
            {
                yield return record;
            }
        } 

        public override HtmlTag ToLeafTag()
        {
            var tag = new HtmlTag("li").Text(Description);
            var root = tag.Add("ol");

            allRecordNodes()
                .OrderBy(x => x.Timestamp)
                .Select(x => x.ToLeafTag())
                .Each(x => root.Append(x));

            return tag;
        }

        public void Record(MessageRecord record)
        {
            if (Type.IsEmpty() && record.Type.IsNotEmpty())
            {
                Type = record.Type;
            }

            _records.Add(record);
        }
    }
}