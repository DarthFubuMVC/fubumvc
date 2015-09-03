using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightningQueues.Exceptions;
using LightningQueues.Logging;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public abstract class AbstractActions : IDisposable
    {
        protected readonly ILogger logger;
        protected readonly Guid instanceId;
		protected readonly ColumnsInformation ColumnsInformation;
    	protected JET_DBID dbid;
        protected EsentTable queues;
		protected EsentTable subqueues;
        protected EsentTable recovery;
		protected Session session;
        protected Transaction transaction;
        protected EsentTable txs;
        protected EsentTable outgoing;
        protected EsentTable outgoingHistory;
		protected EsentTable recveivedMsgs;

        protected readonly Dictionary<string, QueueActions> queuesByName = new Dictionary<string, QueueActions>();

		protected AbstractActions(JET_INSTANCE instance, ColumnsInformation columnsInformation, string database, Guid instanceId)
		{
		    logger = LogManager.GetLogger(GetType());
		    try
			{
				this.instanceId = instanceId;
				ColumnsInformation = columnsInformation;
				session = new Session(instance);

				transaction = new Transaction(session);
				Api.JetOpenDatabase(session, database, null, out dbid, OpenDatabaseGrbit.None);

				queues = new EsentTable(session, new Table(session, dbid, "queues", OpenTableGrbit.None));
				subqueues = new EsentTable(session, new Table(session, dbid, "subqueues", OpenTableGrbit.None));
				txs = new EsentTable(session, new Table(session, dbid, "transactions", OpenTableGrbit.None));
				recovery = new EsentTable(session, new Table(session, dbid, "recovery", OpenTableGrbit.None));
				outgoing = new EsentTable(session, new Table(session, dbid, "outgoing", OpenTableGrbit.None));
				outgoingHistory = new EsentTable(session, new Table(session, dbid, "outgoing_history", OpenTableGrbit.None));
				recveivedMsgs = new EsentTable(session, new Table(session, dbid, "recveived_msgs", OpenTableGrbit.None));
			}
			catch (Exception)
			{
				Dispose();
				throw;
			}
		}

        public void ClearAllMessages()
        {
            ClearTable(outgoing);
            ClearTable(outgoingHistory);
            ClearTable(recveivedMsgs);
            ClearTable(recovery);
            ClearTable(txs);
        }

        internal void ClearTable(EsentTable table)
        {
            var enumerator = table.GetEnumerator();
            while (enumerator.MoveNext())
            {
                table.Delete();
            }
        }

        public QueueActions GetQueue(string queueName)
        {
            QueueActions actions;
            if (queuesByName.TryGetValue(queueName, out actions))
                return actions;

            var enumerator = queues.GetEnumerator(new StringValueIndex("pk", queueName));

            if(!enumerator.MoveNext())
                throw new QueueDoesNotExistsException(queueName);

            queuesByName[queueName] = actions =
				new QueueActions(session, dbid, queueName, GetSubqueues(queueName), this,
					i => AddToNumberOfMessagesIn(queueName, i));
            return actions;
        }

		private string[] GetSubqueues(string queueName)
		{
			var list = new List<string>();
		    var enumerator = subqueues.GetEnumerator(new StringValueIndex("by_queue", queueName));

		    while (enumerator.MoveNext())
		    {
		        list.Add(subqueues.ForColumnType<StringColumn>().Get("subqueue"));
		    }

			return list.ToArray();
		}

		public void AddSubqueueTo(string queueName, string subQueue)
		{
			try
			{
			    subqueues.Insert(() =>
			    {
			        subqueues.ForColumnType<StringColumn>().Set("queue", queueName);
			        subqueues.ForColumnType<StringColumn>().Set("subqueue", subQueue);
			    });
			}
			catch (EsentErrorException e)
			{
				if (e.Error != JET_err.KeyDuplicate)
					throw;
			}
		}

        private void AddToNumberOfMessagesIn(string queueName, int count)
        {
            var enumerator = queues.GetEnumerator(new StringValueIndex("pk", queueName));

            if (!enumerator.MoveNext())
                return;

            queues.ForColumnType<IntColumn>().InterlockedIncrement("number_of_messages", count);
        }

        public void Dispose()
        {
        	try
        	{
        		foreach (var action in queuesByName.Values)
        		{
        			action.Dispose();
        		}

        		if (queues != null)
        			queues.Dispose();
        		if (subqueues != null)
        			subqueues.Dispose();
        		if (txs != null)
        			txs.Dispose();
        		if (recovery != null)
        			recovery.Dispose();
        		if (outgoing != null)
        			outgoing.Dispose();
        		if (outgoingHistory != null)
        			outgoingHistory.Dispose();
        		if (recveivedMsgs != null)
        			recveivedMsgs.Dispose();

        		if (Equals(dbid, JET_DBID.Nil) == false)
        			Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);

        		if (transaction != null)
        			transaction.Dispose();

        		if (session != null)
        			session.Dispose();
        	}
        	catch (Exception e)
        	{
				logger.Error("Error occurred while disposing " + GetType().Name, e);
        	}
        }

    	public void Commit()
        {
            transaction.Commit(CommitTransactionGrbit.None);
        }
   }
}
