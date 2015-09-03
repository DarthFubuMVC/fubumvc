using System;
using LightningQueues.Utils;

namespace LightningQueues.Model
{
    public class MessageId
    {
        public Guid SourceInstanceId { get; set; }
        public Guid MessageIdentifier { get; set; }

        public static MessageId GenerateRandom()
        {
            return new MessageId
            {
                SourceInstanceId = Guid.NewGuid(),
                MessageIdentifier = GuidCombGenerator.Generate()
            };
        }

    	public bool Equals(MessageId other)
    	{
    		if (ReferenceEquals(null, other)) return false;
    		if (ReferenceEquals(this, other)) return true;
    		return other.SourceInstanceId.Equals(SourceInstanceId) && other.MessageIdentifier.Equals(MessageIdentifier);
    	}

    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(null, obj)) return false;
    		if (ReferenceEquals(this, obj)) return true;
    		if (obj.GetType() != typeof (MessageId)) return false;
    		return Equals((MessageId) obj);
    	}

    	public override int GetHashCode()
    	{
    		unchecked
    		{
    			return (SourceInstanceId.GetHashCode()*397) ^ MessageIdentifier.GetHashCode();
    		}
    	}

    	public override string ToString()
        {
            return string.Format("{0}/{1}", SourceInstanceId, MessageIdentifier);
        }
    }
}
