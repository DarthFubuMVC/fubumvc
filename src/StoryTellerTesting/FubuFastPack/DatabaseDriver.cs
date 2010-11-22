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
        public FakeDomainNHIbernateRegistry()
        {
            MappingFromThisAssembly();
        }
    }

    public static class DatabaseDriver
    {
        private static readonly string FILE_NAME = "test.db";
        private static IContainer _container;

        public static void Bootstrap()
        {
            if (_container != null) return;

            if (File.Exists(FILE_NAME))
            {
                File.Delete(FILE_NAME);
            }

            // TODO -- switch this to in-memory
            var settings = new DatabaseSettings(){
                ConnectionString = "Data Source={0};Version=3;New=True;".ToFormat(FILE_NAME),
                DialectType = typeof (SQLiteDialect),
                ProxyFactoryType = typeof (ProxyFactoryFactory),
                ShowSql = true,
                DriverType = typeof(SQLite20Driver)
            };

            _container = new Container(x =>
            {
                x.For<DatabaseSettings>().Use(settings);
                x.BootstrapNHibernate<FakeDomainNHIbernateRegistry>(ConfigurationBehavior.AlwaysUseNewConfiguration);
            });



            var writer = _container.GetInstance<ISchemaWriter>();
            writer.BuildSchema();
        }

        public static IContainer TransactionalContainer()
        {
            return _container.GetNestedContainer();
        }
    }
}