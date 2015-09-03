using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public class QueueSchemaCreator
    {
        private readonly Session session;
        private readonly JET_DBID dbid;
        private readonly string queueName;

        public QueueSchemaCreator(Session session, JET_DBID dbid, string queueName)
        {
            this.session = session;
            this.dbid = dbid;
            this.queueName = queueName;
        }

        public void Create()
        {
            CreateMessagesTable();
            CreateMessagesHistoryTable();
        }

        private void CreateMessagesHistoryTable()
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(session, dbid, queueName +"_history", 16, 100, out tableid);
            JET_COLUMNID columnid;
            Api.JetAddColumn(session, tableid, "local_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "instance_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Binary,
                cbMax = 16,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "msg_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Binary,
                cbMax = 16,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "timestamp", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.DateTime,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "moved_to_history_at", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.DateTime,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "headers", new JET_COLUMNDEF
            {
                cbMax = 8192,
                cp = JET_CP.Unicode,
                coltyp = JET_coltyp.LongText,
                grbit = ColumndefGrbit.None
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "data", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.LongBinary,
                // For Win2k3 support, it doesn't support long binary columsn that are not null
                grbit = ColumndefGrbit.None
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "status", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "subqueue", new JET_COLUMNDEF
            {
                cbMax = 255,
                cp = JET_CP.Unicode,
                coltyp = JET_coltyp.Text,
                grbit = ColumndefGrbit.None
            }, null, 0, out columnid);

            string indexDef = "+subqueue\0\0";
            Api.JetCreateIndex(session, tableid, "by_sub_queue", CreateIndexGrbit.None, indexDef, indexDef.Length,
                               100);
            indexDef = "+msg_id\0\0";
            Api.JetCreateIndex(session, tableid, "msg_id", CreateIndexGrbit.IndexUnique | CreateIndexGrbit.IndexDisallowNull, indexDef, indexDef.Length,
                               100);
        }

        private void CreateMessagesTable()
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(session, dbid, queueName, 16, 100, out tableid);
            JET_COLUMNID columnid;

            Api.JetAddColumn(session, tableid, "local_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL | ColumndefGrbit.ColumnAutoincrement
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "instance_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Binary,
                cbMax = 16,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "msg_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Binary,
                cbMax = 16,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "timestamp", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.DateTime,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "headers", new JET_COLUMNDEF
            {
                cbMax = 8192,
                cp = JET_CP.Unicode,
                coltyp = JET_coltyp.LongText,
                grbit = ColumndefGrbit.None
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "data", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.LongBinary,
                // For Win2k3 support, it doesn't support long binary columsn that are not null
                grbit = ColumndefGrbit.None
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "status", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "subqueue", new JET_COLUMNDEF
            {
                cbMax = 255,
                cp = JET_CP.Unicode,
                coltyp = JET_coltyp.Text,
                grbit = ColumndefGrbit.None
            }, null, 0, out columnid);

            var indexDef = "+local_id\0\0";
            Api.JetCreateIndex(session, tableid, "pk", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length,
                               100);

			indexDef = "+subqueue\0+timestamp\0\0";
            Api.JetCreateIndex(session, tableid, "by_sub_queue", CreateIndexGrbit.None, indexDef, indexDef.Length,
                               100);

            indexDef = "+instance_id\0+msg_id\0\0";
            Api.JetCreateIndex(session, tableid, "by_id", CreateIndexGrbit.None, indexDef, indexDef.Length,
                               100);
        }
        
    }
}
