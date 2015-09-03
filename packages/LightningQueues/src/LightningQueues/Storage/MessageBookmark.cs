using System;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public class MessageBookmark : IEquatable<MessageBookmark>
    {
        public string QueueName;
        public byte[] Bookmark = new byte[SystemParameters.BookmarkMost];
        public int Size = SystemParameters.BookmarkMost;

        public bool Equals(MessageBookmark other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (!string.Equals(QueueName, other.QueueName) || Size != other.Size)
            {
                return false;
            }

            for (int i = 0; i < Size; i++)
            {
                if (Bookmark[i] != other.Bookmark[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MessageBookmark);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = QueueName.GetHashCode();
                hashCode = (hashCode * 397) ^ Size;
                hashCode = (hashCode * 397) ^ Bookmark.GetHashCode();
                return hashCode;
            }
        }
    }
}