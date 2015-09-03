using System;
using System.Text;
using LightningQueues.Model;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public interface IEsentIndex
    {
        Session Session { get; set; }
        Table Table { get; set; }
        bool SeekTo();
    }

    public class StringValueIndex : IndexBase
    {
        private readonly string _value;

        public StringValueIndex(string indexName, string value) : base(indexName)
        {
            _value = value;
        }

        public StringValueIndex(string value)
        {
            _value = value;
        }

        protected override bool SeekImpl()
        {
            Api.MakeKey(Session, Table, _value, Encoding.Unicode, MakeKeyGrbit.NewKey);

            if (Api.TrySeek(Session, Table, SeekGrbit.SeekGE) == false)
                return false;

            Api.MakeKey(Session, Table, _value, Encoding.Unicode, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
            try
            {
                Api.JetSetIndexRange(Session, Table, SetIndexRangeGrbit.RangeInclusive | SetIndexRangeGrbit.RangeUpperLimit);
            }
            catch (EsentErrorException e)
            {
                if (e.Error != JET_err.NoCurrentRecord)
                    throw;
                return false;
            }
            return true;
        }
    }

    public class StartingIndex : IndexBase
    {
        protected override bool SeekImpl()
        {
            Api.MoveBeforeFirst(Session, Table);
            return Api.TryMoveNext(Session, Table);
        }
    }

    public class MessageIdIndex : IndexBase
    {
        private readonly MessageId _id;

        public MessageIdIndex(MessageId id) : base("by_id")
        {
            _id = id;
        }

        protected override bool SeekImpl()
        {
            Api.MakeKey(Session, Table, _id.SourceInstanceId.ToByteArray(), MakeKeyGrbit.NewKey);
            Api.MakeKey(Session, Table, _id.MessageIdentifier, MakeKeyGrbit.None);

            if (Api.TrySeek(Session, Table, SeekGrbit.SeekEQ) == false)
                return false;

            Api.MakeKey(Session, Table, _id.SourceInstanceId.ToByteArray(), MakeKeyGrbit.NewKey);
            Api.MakeKey(Session, Table, _id.MessageIdentifier, MakeKeyGrbit.None);
            try
            {
                Api.JetSetIndexRange(Session, Table,
                    SetIndexRangeGrbit.RangeInclusive | SetIndexRangeGrbit.RangeUpperLimit);
            }
            catch (EsentErrorException e)
            {
                if (e.Error != JET_err.NoCurrentRecord)
                    throw;
                return false;
            }
            return true;
        }
    }

    public class PositionalIndexFromLast : IndexBase
    {
        private readonly int _numberOfItemsFromEnd;

        public PositionalIndexFromLast(int numberOfItemsFromEnd)
        {
            _numberOfItemsFromEnd = numberOfItemsFromEnd;
        }

        protected override bool SeekImpl()
        {
            Api.MoveAfterLast(Session, Table);
            try
            {
                Api.JetMove(Session, Table, -_numberOfItemsFromEnd, MoveGrbit.None);
            }
            catch (EsentErrorException e)
            {
                if (e.Error == JET_err.NoCurrentRecord)
                    return false;
                throw;
            }
            return true;
        }
    }

    public class GuidIndex : IndexBase
    {
        private readonly Guid _value;

        public GuidIndex(Guid value)
        {
            _value = value;
        }

        public GuidIndex(Guid value, string indexName) : base(indexName)
        {
            _value = value;
        }

        protected override bool SeekImpl()
        {
            Api.MakeKey(Session, Table, _value.ToByteArray(), MakeKeyGrbit.NewKey);
            var found = Api.TrySeek(Session, Table, SeekGrbit.SeekEQ);
            if (found)
            {
                Api.MakeKey(Session, Table, _value.ToByteArray(), MakeKeyGrbit.NewKey);
                try
                {
                    Api.JetSetIndexRange(Session, Table, SetIndexRangeGrbit.RangeInclusive | SetIndexRangeGrbit.RangeUpperLimit);
                }
                catch (EsentErrorException e)
                {
                    if (e.Error != JET_err.NoCurrentRecord)
                        throw;
                    return false;
                }
            }
            return found;
        }
    }

    public class BookmarkIndex : IndexBase
    {
        private readonly int _size;
        private readonly byte[] _bookmark;

        public BookmarkIndex(int size, byte[] bookmark) : base("by_bookmark")
        {
            _size = size;
            _bookmark = bookmark;
        }

        protected override bool SeekImpl()
        {
            Api.MakeKey(Session, Table, _size, MakeKeyGrbit.NewKey);
            Api.MakeKey(Session, Table, _bookmark, MakeKeyGrbit.None);
            return Api.TrySeek(Session, Table, SeekGrbit.SeekEQ);
        }
    }

    public abstract class IndexBase : IEsentIndex
    {
        private readonly Action _setIndex;

        protected IndexBase(string indexName)
        {
            _setIndex = () => Api.JetSetCurrentIndex(Session, Table, indexName);
        }

        protected IndexBase()
        {
            _setIndex = () => { };
        }

        public Session Session { get; set; }
        public Table Table { get; set; }

        public bool SeekTo()
        {
            _setIndex();
            return SeekImpl();
        }
        protected abstract bool SeekImpl();
    }
}