using System;
using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FubuCore.Binding;
using NHibernate.Connection;

namespace FubuFastPack.NHibernate
{
    // TODO -- validate this on set up
    public class DatabaseSettings
    {
        public DatabaseSettings()
        {
            ProviderType = typeof (DriverConnectionProvider);
        }

        public string Provider { get; set; }
        public string Driver { get; set; }
        public string Dialect { get; set; }
        public bool UseOuterJoin { get; set; }
        [ConnectionString]
        public string ConnectionString { get; set; }
        public bool ShowSql { get; set; }
        public string ProxyFactory { get; set; }
        public string CacheProvider { get; set; }
        public bool GenerateStatistics { get; set; }
        public string ConnectionProvider { get; set; }

        public bool IsCacheEnabled()
        {
            return !string.IsNullOrEmpty(CacheProvider);
        }

        public Type DriverType
        {
            set
            {
                Driver = value.FullName;
            }
        }

        public Type ConnectionProviderType
        {
            set
            {
                ConnectionProvider = value.FullName;
            }
        }

        public Type ProviderType
        {
            set
            {
                Provider = value.FullName;
            }
        }

        public Type DialectType
        {
            set
            {
                Dialect = value.FullName;
            }
        }

        public Type ProxyFactoryType
        {
            set
            {
                ProxyFactory = value.AssemblyQualifiedName;
            }
        }

        public IDictionary<string, string> GetProperties()
        {
            

            if (Provider.IsEmpty()) throw new ApplicationException("DatabaseSettings unavailable. Make sure your application configuration file has appSetting entries for the necessary DatabaseSettings properties.");
            var properties = new Dictionary<string, string>
                             {
                                 {"connection.provider", Provider},
                                 {"connection.driver_class", Driver},
                                 {"dialect", Dialect},
                                 {"use_outer_join", UseOuterJoin.ToString().ToLowerInvariant()},
                                 {"connection.connection_string", ConnectionString},
                                 {"show_sql", ShowSql.ToString().ToLowerInvariant()},
                                 {"proxyfactory.factory_class", ProxyFactory}
                             };

            if (IsCacheEnabled())
            {
                properties.Add("cache.provider_class", CacheProvider);
                properties.Add("cache.use_second_level_cache", "true");
                properties.Add("cache.use_query_cache", "true");
            }

            if (GenerateStatistics)
            {
                properties.Add("generate_statistics", "true");
            }


            return properties;
        }
    }
}