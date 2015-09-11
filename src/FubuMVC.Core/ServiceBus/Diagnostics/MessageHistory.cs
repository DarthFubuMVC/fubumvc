using System.Collections.Generic;
using System.Linq;
using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public class MessageHistory : MessageRecordNode
    {
        private readonly IList<MessageHistory> _children = new List<MessageHistory>();
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

        public void AddChild(MessageHistory child)
        {
            _children.Fill(child);
            child.Parent = this;
        }

        public MessageHistory Parent { get; set; }


        protected bool Equals(MessageHistory other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MessageHistory) obj);
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