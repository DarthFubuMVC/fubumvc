using System;
using System.Diagnostics;
using System.IO;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap;
using FubuTestApplication.Domain;
using NHibernate.Dialect;
using NHibernate.Driver;
using FubuCore;
using StructureMap;
using FubuFastPack.StructureMap;

namespace FubuTestApplication
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
            MappingsFromAssembly(typeof(Case).Assembly);
        }
    }

    public static class DatabaseDriver
    {
        private static IContainer _container;
        private static readonly string FILE_NAME;
        private static DatabaseSettings _settings;

        static DatabaseDriver()
        {
            FILE_NAME = FileSystem.Combine(FubuMvcPackageFacility.GetApplicationPath(), "test.db");
        }

        public static void Bootstrap(bool cleanFile)
        {
            if (_container != null) return;


            _container = BootstrapContainer(FILE_NAME, true);
        }

        public static IContainer BootstrapContainer(string fileName, bool cleanFile)
        {
            if (File.Exists(fileName) && cleanFile)
            {
                File.Delete(fileName);
            }

            _settings = new DatabaseSettings(){
                ConnectionString = "Data Source={0};Version=3;New=True;".ToFormat(fileName),
                DialectType = typeof (SQLiteDialect),
                ProxyFactory = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle",
                ShowSql = true,
                DriverType = typeof(SQLite20Driver)
            };
            
            return new Container(x =>
            {
                x.AddRegistry(new FastPackRegistry());

                x.For<DatabaseSettings>().Use(_settings);
                x.BootstrapNHibernate<FakeDomainNHIbernateRegistry>(ConfigurationBehavior.AlwaysUseNewConfiguration);
                x.UseExplicitNHibernateTransactionBoundary();

                x.FubuValidationWith(IncludePackageAssemblies.No);
            });
        }

        public static IContainer GetFullFastPackContainer()
        {
            var file = FileSystem.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../test.db").ToFullPath();

            _container = BootstrapContainer(file, true);
            _container.Configure(x =>
            {
                x.AddRegistry(new FastPackRegistry());
                x.For<IObjectConverter>().Use<ObjectConverter>();
            });


            FubuApplication.For<FubuTestApplicationRegistry>()
                .StructureMap(() => _container)
                .Packages(x => x.Assembly(typeof(FastPackRegistry).Assembly)).Bootstrap();

            _container.GetInstance<ISchemaWriter>().BuildSchema();

            return _container;
        }

        public static IContainer BuildWebsiteContainer()
        {
            Bootstrap(true);

            return _container;
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