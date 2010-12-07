using FubuFastPack.NHibernate;
using NUnit.Framework;
using StructureMap;
using FubuMVC.Tests;

namespace IntegrationTesting.FubuFastPack.NHibernate
{
    [TestFixture]
    public class SqlRunnerSmokeTester
    {
        private IContainer container;

        [SetUp]
        public void SetUp()
        {
            DatabaseDriver.Bootstrap();

            container = DatabaseDriver.ContainerWithDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void try_to_run_sql()
        {
            var sqlRunner = container.GetInstance<ISqlRunner>();
            sqlRunner.ExecuteCommand("delete from site");
            sqlRunner.ExecuteScalarCommand<long>("select count(*) from site")
                .ShouldEqual(0);
        }
    }
}