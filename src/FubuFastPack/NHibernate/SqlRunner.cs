using System;
using System.Data;
using NHibernate;

namespace FubuFastPack.NHibernate
{
    public interface ISqlRunner
    {
        TResult ExecuteScalarCommand<TResult>(string sqlCommandText);
        void ExecuteCommand(string sqlCommandText);
    }

    public class SqlRunner : ISqlRunner
    {
        private readonly ISession _session;

        public SqlRunner(ISession session)
        {
            _session = session;
        }

        public TResult ExecuteScalarCommand<TResult>(string sqlCommandText)
        {
            using (var cmd = _session.Connection.CreateCommand())
            {
                _session.Transaction.Enlist(cmd);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCommandText;

                return (TResult) cmd.ExecuteScalar();
            }
        }

        public void ExecuteCommand(string sqlCommandText)
        {
            using (var cmd = _session.Connection.CreateCommand())
            {
                _session.Transaction.Enlist(cmd);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCommandText;

                cmd.ExecuteNonQuery();
            }
        }
    }
}