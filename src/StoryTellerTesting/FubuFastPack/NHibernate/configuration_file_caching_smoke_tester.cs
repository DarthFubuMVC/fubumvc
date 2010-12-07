using System.IO;
using FubuFastPack.NHibernate;
using NUnit.Framework;
using StructureMap;
using FubuMVC.Tests;

namespace IntegrationTesting.FubuFastPack.NHibernate
{
    [TestFixture]
    public class configuration_file_caching_smoke_tester
    {
        private IContainer container;
        private IConfigurationSource source;

        [SetUp]
        public void SetUp()
        {
            SchemaWriter.RemovePersistedConfiguration();
            
            DatabaseDriver.Bootstrap();

            container = DatabaseDriver.ContainerWithDatabase();
            source = container.GetInstance<IConfigurationSource>();
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void write_the_configuration_file()
        {
            container.GetInstance<ISchemaWriter>().WriteConfigurationFile();

            File.Exists(SchemaWriter.NHIBERNATE_CONFIGURATION_FILE);

            var source = DatabaseDriver.ContainerSetToFileCachedConfiguration().GetInstance<IConfigurationSource>();
            source.ShouldBeOfType<FileCacheConfigurationSource>();

            source.Configuration().ShouldNotBeNull();
        }
    }
}