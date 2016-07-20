using System;
using System.Collections.Generic;
using FubuCore.Logging;
using Marten;
using Marten.Services;
using Npgsql;
using NpgsqlTypes;
using StructureMap;

namespace FubuMVC.Marten
{
    public class MartenStructureMapServices : Registry
    {
        public MartenStructureMapServices()
        {
            ForSingletonOf<IDocumentStore>().UseIfNone<DocumentStore>();
            For<IQuerySession>().UseIfNone("Build a QuerySession with logging", c =>
            {
                var session = c.GetInstance<IDocumentStore>().QuerySession();
                session.Logger = c.GetInstance<IMartenSessionLogger>();

                return session;
            });

            For<IDocumentSession>().UseIfNone(c => c.GetInstance<ISessionBoundary>().Session());
            For<ISessionBoundary>().UseIfNone<SessionBoundary>();
            For<ITransaction>().UseIfNone<MartenTransaction>();


            ForSingletonOf<IPersistenceReset>().UseIfNone<MartenPersistenceReset>();
            ForSingletonOf<ICompleteReset>().UseIfNone<CompleteReset>();
            For<IInitialState>().UseIfNone<NulloInitialState>();
            For<IMartenSessionLogger>().UseIfNone<NulloMartenLogger>();
        }

    }


    public class CommandRecordingLogger : IMartenSessionLogger
    {
        private readonly ILogger _logger;

        public CommandRecordingLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void LogSuccess(NpgsqlCommand command)
        {
            CommandCount++;
            _logger.InfoMessage(new CommandExecuted(command));
        }

        public void LogFailure(NpgsqlCommand command, Exception ex)
        {
            CommandCount++;
            _logger.InfoMessage(new CommandFailed(command, ex));
        }

        public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
        {
            
        }

        public int CommandCount { get; private set; }

        public void RecordSavedChanges(IDocumentSession session)
        {
            
        }
    }


    public class CommandExecuted : LogTopic
    {
        public CommandExecuted(NpgsqlCommand command)
        {
            CommandText = command.CommandText;
            Args = new Dictionary<string, object>();
            command.Parameters.Each(param =>
            {
                if (param.NpgsqlDbType == NpgsqlDbType.Jsonb || param.NpgsqlDbType == NpgsqlDbType.Json)
                {
                    Args.Add(param.ParameterName, "[Json Data]");
                }
                else
                {
                    Args.Add(param.ParameterName, param.Value);
                }
            });
        }

        public Dictionary<string, object> Args { get; set; }

        public string CommandText { get; set; }
    }

    public class CommandFailed : CommandExecuted
    {
        public CommandFailed(NpgsqlCommand command, Exception e) : base(command)
        {
            ExceptionText = e.ToString();
        }

        public string ExceptionText { get; set; }
    }
}