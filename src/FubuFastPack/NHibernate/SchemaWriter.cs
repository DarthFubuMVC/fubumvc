using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace FubuFastPack.NHibernate
{
    public interface ISchemaWriter
    {
        void BuildSchema();
        void ExportMappingsTo(string mappingPath);
        void WriteConfigurationFile();
        Configuration Configuration { get; }
    }

    public class SchemaWriter : ISchemaWriter
    {
        public static readonly string NHIBERNATE_CONFIGURATION_FILE = "NHibernateConfiguration.bin";

        private readonly IConfigurationSource _source;
        private readonly DatabaseSettings _databaseSettings;

        public SchemaWriter(IConfigurationSource source, DatabaseSettings databaseSettings)
        {
            _source = source;
            _databaseSettings = databaseSettings;
        }

        public void BuildSchema()
        {
            using (var session = _source.Configuration().BuildSessionFactory().OpenSession())
            {
                var connection = session.Connection;
                var configuration = _source.Configuration();

                var dialect = Dialect.GetDialect(_databaseSettings.GetProperties());
                var drops = configuration.GenerateDropSchemaScript(dialect);

                executeScripts(drops, connection);

                var scripts = configuration.GenerateSchemaCreationScript(dialect);
                executeScripts(scripts, connection);
            }
            

        }

        public Configuration Configuration
        {
            get
            {
                return _source.Configuration();
            }
        }

        public void ExportMappingsTo(string mappingPath)
        {
            if (Directory.Exists(mappingPath))
            {
                Directory.Delete(mappingPath, true);
            }
            Directory.CreateDirectory(mappingPath);

            _source.Model().WriteMappingsTo(mappingPath);
        }

        public void WriteConfigurationFile()
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(NHIBERNATE_CONFIGURATION_FILE, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, _source.Configuration());
                stream.Close();
            }
        }

        public static void RemovePersistedConfiguration()
        {
            if (File.Exists(NHIBERNATE_CONFIGURATION_FILE))
            {
                File.Delete(NHIBERNATE_CONFIGURATION_FILE);
            }
        }

        private static void executeScripts(IEnumerable<string> scripts, IDbConnection connection)
        {
            foreach (var script in scripts)
            {
                var command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();
            }
        }
    }
}