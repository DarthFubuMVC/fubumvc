using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentNHibernate;
using NHibernate.Cfg;

namespace FubuFastPack.NHibernate
{
    public class FileCacheConfigurationSource : IConfigurationSource
    {
        private readonly IConfigurationSource _inner;

        public FileCacheConfigurationSource(IConfigurationSource inner)
        {
            _inner = inner;
        }

        public Configuration Configuration()
        {
            var formatter = new BinaryFormatter();

            if (File.Exists(SchemaWriter.NHIBERNATE_CONFIGURATION_FILE))
            {
                
                using (var stream = new FileStream(SchemaWriter.NHIBERNATE_CONFIGURATION_FILE, FileMode.Open, FileAccess.Read))
                {
                    return (Configuration)formatter.Deserialize(stream);
                }
            }

            var configuration = _inner.Configuration();

            using (var stream = new FileStream(SchemaWriter.NHIBERNATE_CONFIGURATION_FILE, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, configuration);
            }

            return configuration;
        }

        public PersistenceModel Model()
        {
            return _inner.Model();
        }

    }
}