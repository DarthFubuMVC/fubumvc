using System;
using System.Web;
using LightningQueues.Model;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public static class EsentExtension
    {
        public static void WithDatabase(this JET_INSTANCE instance, string database, Action<Session, JET_DBID> action)
        {
            using (var session = new Session(instance))
            {
                JET_DBID dbid;
                Api.JetOpenDatabase(session, database, "", out dbid, OpenDatabaseGrbit.None);
                try
                {
                    action(session, dbid);
                }
                finally
                {
                    Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);
                }
            }
        }

        internal static TMessage ReadMessageWithId<TMessage>(this EsentTable table, MessageBookmark bookmark, string queueName) where TMessage : PersistentMessage, new()
        {
            return ReadMessage<TMessage>(table, bookmark, queueName, x =>
            {
                x.Id = table.GetMessageId();
            });
        }

        internal static TMessage ReadMessageWithId<TMessage>(this EsentTable table, MessageBookmark bookmark,
            string queueName, Action<TMessage> action) where TMessage : PersistentMessage, new()
        {
            var message = table.ReadMessageWithId<TMessage>(bookmark, queueName);
            action(message);
            return message;
        }

        internal static TMessage ReadMessage<TMessage>(this EsentTable table, MessageBookmark bookmark, string queueName, Action<TMessage> action) where TMessage : PersistentMessage, new()
        {
            var message = new TMessage
            {
                Bookmark = bookmark,
                Status = (MessageStatus)table.ForColumnType<IntColumn>().Get("status"),
                Headers = HttpUtility.ParseQueryString(table.ForColumnType<StringColumn>().Get("headers")),
                Queue = queueName,
                SentAt = table.ForColumnType<DateTimeColumn>().Get("timestamp"),
                Data = table.ForColumnType<BytesColumn>().Get("data"),
            };
            action(message);
            return message;
        }

        internal static MessageId GetMessageId(this EsentTable table)
        {
            var id = new MessageId
            {
                MessageIdentifier = table.ForColumnType<GuidColumn>().Get("msg_id"),
                SourceInstanceId = table.ForColumnType<GuidColumn>().Get("instance_id")
            };
            return id;
        }
    }
}