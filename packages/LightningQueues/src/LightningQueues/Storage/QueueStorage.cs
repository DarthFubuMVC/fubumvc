using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using LightningQueues.Logging;
using LightningQueues.Model;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
	public class QueueStorage : CriticalFinalizerObject, IDisposable
	{
		private JET_INSTANCE _instance;
	    private readonly string _database;
	    private readonly string _path;
	    private ColumnsInformation _columnsInformation;
	    private readonly QueueManagerConfiguration _configuration;
	    private readonly ILogger _logger = LogManager.GetLogger<QueueStorage>();

	    private readonly ReaderWriterLockSlim _usageLock = new ReaderWriterLockSlim();

		public Guid Id { get; private set; }

		public QueueStorage(string database, QueueManagerConfiguration configuration)
		{
		    _configuration = configuration;
		    _database = database;
		    _path = database;
			if (Path.IsPathRooted(database) == false)
				_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, database);
			_database = Path.Combine(_path, Path.GetFileName(database));
			Api.JetCreateInstance(out _instance, database + Guid.NewGuid());
		}

		public void Initialize()
		{
			ConfigureInstance(_instance);
			try
			{
				Api.JetInit(ref _instance);

				EnsureDatabaseIsCreatedAndAttachToDatabase();

				SetIdFromDb();

				LoadColumnInformation();
			}
			catch (Exception e)
			{
				Dispose();
				throw new InvalidOperationException("Could not open queue: " + _database, e);
			}
		}

		private void LoadColumnInformation()
		{
			_columnsInformation = new ColumnsInformation();
			_instance.WithDatabase(_database, (session, dbid) =>
			{
				using (var table = new Table(session, dbid, "subqueues", OpenTableGrbit.ReadOnly))
				{
					_columnsInformation.SubqueuesColumns = Api.GetColumnDictionary(session, table);
				}
				using (var table = new Table(session, dbid, "outgoing_history", OpenTableGrbit.ReadOnly))
				{
					_columnsInformation.OutgoingHistoryColumns = Api.GetColumnDictionary(session, table);
				}
				using (var table = new Table(session, dbid, "outgoing", OpenTableGrbit.ReadOnly))
				{
					_columnsInformation.OutgoingColumns = Api.GetColumnDictionary(session, table);
				}
				using (var table = new Table(session, dbid, "recovery", OpenTableGrbit.ReadOnly))
				{
					_columnsInformation.RecoveryColumns = Api.GetColumnDictionary(session, table);
				}
				using (var table = new Table(session, dbid, "transactions", OpenTableGrbit.ReadOnly))
				{
					_columnsInformation.TxsColumns = Api.GetColumnDictionary(session, table);
				}
				using (var table = new Table(session, dbid, "queues", OpenTableGrbit.ReadOnly))
				{
					_columnsInformation.QueuesColumns = Api.GetColumnDictionary(session, table);
				}
				using (var table = new Table(session, dbid, "recveived_msgs", OpenTableGrbit.ReadOnly))
				{
					_columnsInformation.RecveivedMsgsColumns = Api.GetColumnDictionary(session, table);
				}
			});
		}

		private void ConfigureInstance(JET_INSTANCE jetInstance)
		{
			new InstanceParameters(jetInstance)
			{
				CircularLog = true,
				Recovery = true,
				CreatePathIfNotExist = true,
				TempDirectory = Path.Combine(_path, "temp"),
				SystemDirectory = Path.Combine(_path, "system"),
				LogFileDirectory = Path.Combine(_path, "logs"),
				MaxVerPages = 8192,
				MaxTemporaryTables = 8192
			};
		}

		private void SetIdFromDb()
		{
			try
			{
				_instance.WithDatabase(_database, (session, dbid) =>
				{
					using (var details = new Table(session, dbid, "details", OpenTableGrbit.ReadOnly))
					{
						Api.JetMove(session, details, JET_Move.First, MoveGrbit.None);
						var columnids = Api.GetColumnDictionary(session, details);
						var column = Api.RetrieveColumn(session, details, columnids["id"]);
						Id = new Guid(column);
						var schemaVersion = Api.RetrieveColumnAsString(session, details, columnids["schema_version"]);
						if (schemaVersion != SchemaCreator.SchemaVersion)
							throw new InvalidOperationException("The version on disk (" + schemaVersion + ") is different that the version supported by this library: " + SchemaCreator.SchemaVersion + Environment.NewLine +
																"You need to migrate the disk version to the library version, alternatively, if the data isn't important, you can delete the file and it will be re-created (with no data) with the library version.");
					}
				});
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Could not read db details from disk. It is likely that there is a version difference between the library and the db on the disk." + Environment.NewLine +
													"You need to migrate the disk version to the library version, alternatively, if the data isn't important, you can delete the file and it will be re-created (with no data) with the library version.", e);
			}
		}

		private void EnsureDatabaseIsCreatedAndAttachToDatabase()
		{
			using (var session = new Session(_instance))
			{
				try
				{
					Api.JetAttachDatabase(session, _database, AttachDatabaseGrbit.None);
					return;
				}
				catch (EsentErrorException e)
				{
					if (e.Error == JET_err.DatabaseDirtyShutdown)
					{
						try
						{
							using (var recoverInstance = new Instance("Recovery instance for: " + _database))
							{
								recoverInstance.Init();
								using (var recoverSession = new Session(recoverInstance))
								{
									ConfigureInstance(recoverInstance.JetInstance);
									Api.JetAttachDatabase(recoverSession, _database,
														  AttachDatabaseGrbit.DeleteCorruptIndexes);
									Api.JetDetachDatabase(recoverSession, _database);
								}
							}
						}
						catch (Exception)
						{
						}

						Api.JetAttachDatabase(session, _database, AttachDatabaseGrbit.None);
						return;
					}
					if (e.Error != JET_err.FileNotFound)
						throw;
				}

				new SchemaCreator(session).Create(_database);
				Api.JetAttachDatabase(session, _database, AttachDatabaseGrbit.None);
			}
		}

		public void Dispose()
		{
			_usageLock.EnterWriteLock();
			try
			{
				_logger.Debug("Disposing queue storage");
				try
				{
					Api.JetTerm2(_instance, TermGrbit.Complete);
					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					_logger.Error("Could not dispose of queue storage properly", e);
					throw;
				}
			}
			finally
			{
				_usageLock.ExitWriteLock();
			}
		}

		~QueueStorage()
		{
			try
			{
				_logger.Info("Disposing esent resources from finalizer! You should call QueueStorage.Dispose() instead!");
				Api.JetTerm2(_instance, TermGrbit.Complete);
			}
			catch (Exception exception)
			{
				try
				{
					_logger.Error("Failed to dispose esent instance from finalizer, trying abrupt termination.", exception);
					try
					{
						Api.JetTerm2(_instance, TermGrbit.Abrupt);
					}
					catch (Exception e)
					{
						_logger.Error("Could not dispose esent instance abruptly", e);
					}
				}
				catch
				{
				}
			}
		}

		public void Global(Action<GlobalActions> action)
		{
	        ScopedAction(GetGlobal, action);
		}

	    public T Global<T>(Func<GlobalActions, T> selector)
	    {
	        return ScopedActions(GetGlobal, selector);
	    }

	    public void Send(Action<SenderActions> action)
	    {
	        ScopedAction(GetSender, action);
	    }

	    private void ScopedAction<T>(Func<T> constructor, Action<T> action) where T : AbstractActions
	    {
	        ScopedActions(constructor, x =>
	        {
	            action(x);
	            return true;
	        });
	    }

	    public T Send<T>(Func<SenderActions, T> selector)
	    {
	        return ScopedActions(GetSender, selector);
	    }

	    private SenderActions GetSender()
	    {
	        return new SenderActions(_instance, _columnsInformation, _database, Id, _configuration);
	    }

	    private GlobalActions GetGlobal()
	    {
	        return new GlobalActions(_instance, _columnsInformation, _database, Id, _configuration);
	    }

	    private TResult ScopedActions<T,TResult>(Func<T> constructor, Func<T, TResult> selector) where T : AbstractActions
	    {
			var shouldTakeLock = _usageLock.IsReadLockHeld == false;
			try
			{
				if (shouldTakeLock)
					_usageLock.EnterReadLock();
				using (var qa = constructor())
				{
					var result = selector(qa);
                    qa.Commit();
				    return result;
				}
			}
			finally
			{
				if (shouldTakeLock)
					_usageLock.ExitReadLock();
			}
	        
	    }

	    public IEnumerable<MessageId> PurgeHistory()
	    {
            _logger.Info("Starting to purge old data");
	        var cleanedUpIds = Enumerable.Empty<MessageId>();
            try
            {
                PurgeProcessedMessages();
                PurgeOutgoingHistory();
                cleanedUpIds = PurgeOldestReceivedMessageIds();
            }
            catch (Exception exception)
            {
                _logger.Info("Failed to purge old data from the system {0}", exception);
            }
	        return cleanedUpIds;
	    }

        private void PurgeProcessedMessages()
        {
            if (!_configuration.EnableProcessedMessageHistory)
                return;

            string[] queues = null;
            Global(actions =>
            {
                queues = actions.GetAllQueuesNames();
            });

            foreach (string queue in queues)
            {
                PurgeProcessedMessagesInQueue(queue);
            }
        }

        private void PurgeProcessedMessagesInQueue(string queue)
        {
            // To make this batchable:
            // 1: Move to the end of the history (to the newest messages) and seek 
            //    backword by NumberOfMessagesToKeepInProcessedHistory.
            // 2: Save a bookmark of the current position.
            // 3: Delete from the beginning of the table (oldest messages) in batches until 
            //    a) we reach the bookmark or b) we hit OldestMessageInProcessedHistory.
            MessageBookmark purgeLimit = null;
            int numberOfMessagesToKeep = _configuration.NumberOfMessagesToKeepInProcessedHistory;
            if (numberOfMessagesToKeep > 0)
            {
                Global(actions =>
                {
                    var queueActions = actions.GetQueue(queue);
                    purgeLimit = queueActions.GetMessageHistoryBookmarkAtPosition(numberOfMessagesToKeep);
                });

                if (purgeLimit == null)
                    return;
            }

            bool foundMessages = false;
            do
            {
                foundMessages = false;
                Global(actions =>
                {
                    var queueActions = actions.GetQueue(queue);
                    var messages = queueActions.GetAllProcessedMessages(batchSize: 250)
                        .TakeWhile(x => (purgeLimit == null || !x.Bookmark.Equals(purgeLimit))
                            && (DateTime.Now - x.SentAt) > _configuration.OldestMessageInProcessedHistory);

                    foreach (var message in messages)
                    {
                        foundMessages = true;
                        _logger.Debug("Purging message {0} from queue {1}/{2}", message.Id, message.Queue, message.SubQueue);
                        queueActions.DeleteHistoric(message.Bookmark);
                    }
                });
            } while (foundMessages);
        }

        private void PurgeOutgoingHistory()
        {
            // Outgoing messages are still stored in the history in case the sender 
            // needs to revert, so there will still be messages to purge even when
            // the QueueManagerConfiguration has disabled outgoing history.
            //
            // To make this batchable:
            // 1: Move to the end of the history (to the newest messages) and seek 
            //    backword by NumberOfMessagesToKeepInOutgoingHistory.
            // 2: Save a bookmark of the current position.
            // 3: Delete from the beginning of the table (oldest messages) in batches until 
            //    a) we reach the bookmark or b) we hit OldestMessageInOutgoingHistory.

            MessageBookmark purgeLimit = null;
            int numberOfMessagesToKeep = _configuration.NumberOfMessagesToKeepInOutgoingHistory;
            if (numberOfMessagesToKeep > 0 && _configuration.EnableOutgoingMessageHistory)
            {
                Global(actions =>
                {
                    purgeLimit = actions.GetSentMessageBookmarkAtPosition(numberOfMessagesToKeep);
                });

                if (purgeLimit == null)
                    return;
            }

            bool foundMessages = false;
            do
            {
                foundMessages = false;
                Global(actions =>
                {
                    IEnumerable<PersistentMessageToSend> sentMessages = actions.GetSentMessages(batchSize: 250)
                        .TakeWhile(x => (purgeLimit == null || !x.Bookmark.Equals(purgeLimit))
                            && (!_configuration.EnableOutgoingMessageHistory || (DateTime.Now - x.SentAt) > _configuration.OldestMessageInOutgoingHistory));

                    foreach (var sentMessage in sentMessages)
                    {
                        foundMessages = true;
                        _logger.Debug("Purging sent message {0} to {1}/{2}/{3}", sentMessage.Id, sentMessage.Endpoint,
                                           sentMessage.Queue, sentMessage.SubQueue);
                        actions.DeleteMessageToSendHistoric(sentMessage.Bookmark);
                    }
                });
            } while (foundMessages);
        }

        private IEnumerable<MessageId> PurgeOldestReceivedMessageIds()
        {
            int totalCount = 0;
            List<MessageId> totalRemoved = new List<MessageId>();
            List<MessageId> deletedMessageIds = new List<MessageId>();
            do
            {
                Global(actions =>
                {
                    deletedMessageIds = actions.DeleteOldestReceivedMessageIds(_configuration.NumberOfReceivedMessageIdsToKeep, 250)
                        .ToList();
                    totalRemoved.AddRange(deletedMessageIds);
                });
                totalCount += deletedMessageIds.Count;
            } while (deletedMessageIds.Count > 0);

            _logger.Info("Purged {0} message ids", totalCount);
            return totalRemoved;
        }

	    public void ClearAllMessages()
	    {
	        Global(actions =>
	        {
	            var queueNames = actions.GetAllQueuesNames();
	            queueNames.Each(queueName =>
	            {
	                var queue = actions.GetQueue(queueName);
	                queue.ClearQueue();
	            });
                actions.ClearAllMessages();
	        });
	    }
	}
}