using System.IO;
using FubuFastPack.NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Dialect;
using NHibernate.Driver;
using FubuCore;
using StructureMap;
using FubuFastPack.StructureMap;

namespace IntegrationTesting.FubuFastPack
{


    /*
        var configuration = new Configuration()  
            .AddProperties(new Dictionary<string, string>  
                               {  
                                   { Environment.ConnectionDriver, typeof( SQLite20Driver ).FullName },  
                                   { Environment.Dialect, typeof( SQLiteDialect ).FullName },  
                                   { Environment.ConnectionProvider, typeof( DriverConnectionProvider ).FullName },  
                                   { Environment.ConnectionString, string.Format( "Data Source={0};Version=3;New=True;", dbFile) },  
                                   { Environment.ProxyFactoryFactoryClass, typeof( ProxyFactoryFactory ).AssemblyQualifiedName },  
                                   { Environment.Hbm2ddlAuto, "create" },  
                                   { Environment.ShowSql, true.ToString() }  
                               });  
     */

    public class FakeDomainNHIbernateRegistry : NHibernateRegistry
    {
        public FakeDomainNHIbernateRegistry(DatabaseSettings settings)
        {
            SetProperties(settings.GetProperties());
            MappingFromThisAssembly();
        }
    }

    public static class DatabaseDriver
    {
        private static IContainer _container;
        private static readonly string FILE_NAME = "test.db";
        private static DatabaseSettings _settings;

        public static void Bootstrap()
        {
            if (_container != null) return;

            if (File.Exists(FILE_NAME))
            {
                File.Delete(FILE_NAME);
            }

            _settings = new DatabaseSettings(){
                ConnectionString = "Data Source={0};Version=3;New=True;".ToFormat(FILE_NAME),
                DialectType = typeof (SQLiteDialect),
                ProxyFactory = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle",
                ShowSql = true,
                DriverType = typeof(SQLite20Driver)
            };

            _container = new Container(x =>
            {
                x.For<DatabaseSettings>().Use(_settings);
                x.BootstrapNHibernate<FakeDomainNHIbernateRegistry>(ConfigurationBehavior.AlwaysUseNewConfiguration);
            });

        }

        public static IContainer ContainerWithoutDatabase()
        {
            return _container.GetNestedContainer();
        }

        public static IContainer ContainerWithDatabase()
        {
            var nested = _container.GetNestedContainer();

            var writer = nested.GetInstance<ISchemaWriter>();
            writer.BuildSchema();

            return nested;
        }

        public static IContainer ContainerSetToFileCachedConfiguration()
        {
            return new Container(x =>
            {
                x.For<DatabaseSettings>().Use(_settings);
                x.BootstrapNHibernate<FakeDomainNHIbernateRegistry>(ConfigurationBehavior.UsePersistedConfigurationIfItExists);
            });
        }
    }
}