using System;
using System.Collections.Generic;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public class EsentTable : IDisposable
    {
        private readonly Session _session;
        private readonly Table _table;
        private readonly IDictionary<string, JET_COLUMNID> _columns;

        public EsentTable(Session session, Table table)
        {
            _session = session;
            _table = table;
            _columns = Api.GetColumnDictionary(session, table);
        }

        public Session Session {get { return _session; }}
        public Table Table {get { return _table; }}

        public IEnumerable<string> ColumnNames {get { return _columns.Keys; }} 

        public IEnumerator<MessageBookmark> GetEnumerator(IEsentIndex index, bool reverse = false)
        {
            index.Session = _session;
            index.Table = _table;
            return new EsentEnumerator(_session, _table, index, reverse);
        }

        public IEnumerator<MessageBookmark> GetEnumerator()
        {
            return GetEnumerator(new StartingIndex());
        }

        public void MoveTo(MessageBookmark bookmark)
        {
			Api.JetGotoBookmark(_session, _table, bookmark.Bookmark, bookmark.Size);
        }

        public MessageBookmark Insert(Action insertBlock)
        {
            using (var update = new Update(_session, _table, JET_prep.Insert))
            {
                insertBlock();
                var bookMark = new MessageBookmark();
                update.Save(bookMark.Bookmark, bookMark.Size, out bookMark.Size);
                return bookMark;
            }
        }

        public MessageBookmark Update(Action updateBlock)
        {
            using (var update = new Update(_session, _table, JET_prep.Replace))
            {
                var bookmark = new MessageBookmark();
                updateBlock();
                update.Save(bookmark.Bookmark, bookmark.Size, out bookmark.Size);
                return bookmark;
            }
        }

        public void Delete()
        {
            Api.JetDelete(_session, _table);
        }

        public T ForColumnType<T>() where T : IColumn, new()
        {
            return new T {Session = _session, Table = _table, Columns = _columns};
        }

        public void Dispose()
        {
            _table.Dispose();
        }
    }
}