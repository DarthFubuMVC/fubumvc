using System.Data;
using NHibernate;

namespace FubuFastPack.NHibernate
{
    public interface ISqlRunner
    {
        TResult ExecuteScalarCommand<TResult>(string sqlCommandText);
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
                //_logger.LogDebug("Execute raw SQL: " + sqlCommandText.Replace(Environment.NewLine, " "));

                return (TResult) cmd.ExecuteScalar();
            }
        }
    }
}