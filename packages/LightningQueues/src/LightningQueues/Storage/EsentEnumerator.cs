using System;
using System.Collections.Generic;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public class EsentEnumerator : IEnumerator<MessageBookmark>
    {
        private readonly Session _session;
        private readonly Table _table;
        private Func<bool> _moveNext;

        public EsentEnumerator(Session session, Table table, IEsentIndex index, bool reverse)
        {
            _session = session;
            _table = table;
            _moveNext = () =>
            {
                var result = index.SeekTo();
                if (reverse)
                    _moveNext = reverseMoveNext;
                else
                    _moveNext = subsequentMoveNext;
                return result;
            };
        }

        public bool MoveNext()
        {
            return _moveNext();
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        MessageBookmark IEnumerator<MessageBookmark>.Current
        {
            get
            {
                var bookmark = new MessageBookmark();
                Api.JetGetBookmark(_session, _table, bookmark.Bookmark, bookmark.Size, out bookmark.Size);
                return bookmark;
            }
        }

        private bool subsequentMoveNext()
        {
            return Api.TryMoveNext(_session, _table);
        }

        private bool reverseMoveNext()
        {
            return Api.TryMovePrevious(_session, _table);
        }

        public void Dispose()
        {
        }
    }
}