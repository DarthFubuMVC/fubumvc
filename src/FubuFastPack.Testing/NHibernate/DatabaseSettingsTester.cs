using FubuFastPack.NHibernate;
using NHibernate.Connection;
using NUnit.Framework;

namespace FubuFastPack.Testing.NHibernate
{
    [TestFixture]
    public class DatabaseSettingsTester
    {
        private DatabaseSettings settings;

        [SetUp]
        public void SetUp()
        {
            settings = new DatabaseSettings();
        }

        [Test]
        public void provider_type_is_set_by_default()
        {
            settings.Provider.ShouldEqual(typeof (DriverConnectionProvider).FullName);
        }

        [Test]
        public void get_the_basic_properties()
        {
            settings.Driver = "driver1";
            settings.Dialect = "dialect1";
            settings.UseOuterJoin = true;
            settings.ConnectionString = "connection string";
            settings.ShowSql = false;
            settings.ProxyFactory = "some proxy";

            var properties = settings.GetProperties();

            properties["connection.driver_class"].ShouldEqual(settings.Driver);
            properties["dialect"].ShouldEqual(settings.Dialect);
            properties["use_outer_join"].ShouldEqual("true");
            properties["connection.connection_string"].ShouldEqual(settings.ConnectionString);
            properties["show_sql"].ShouldEqual("false");
            properties["proxyfactory.factory_class"].ShouldEqual(settings.ProxyFactory);
        }

        [Test]
        public void cache_is_not_enabled()
        {
            settings.CacheProvider = null;

            var properties = settings.GetProperties();
            properties.ContainsKey("cache.provider_class").ShouldBeFalse();
            properties.ContainsKey("cache.use_second_level_cache").ShouldBeFalse();
            properties.ContainsKey("cache.use_query_cache").ShouldBeFalse();
        }

        [Test]
        public void cache_is_enabled()
        {
            settings.CacheProvider = "some cache";

            var properties = settings.GetProperties();
            properties["cache.provider_class"].ShouldEqual("some cache");
            properties["cache.use_second_level_cache"].ShouldEqual("true");
            properties["cache.use_query_cache"].ShouldEqual("true");
        }

        [Test]
        public void statistics_are_not_generated()
        {
            settings.GenerateStatistics = false;
            settings.GetProperties().ContainsKey("generate_statistics").ShouldBeFalse();
        }

        [Test]
        public void statistics_are_generated()
        {
            settings.GenerateStatistics = true;
            settings.GetProperties()["generate_statistics"].ShouldEqual("true");
        }
    }
}