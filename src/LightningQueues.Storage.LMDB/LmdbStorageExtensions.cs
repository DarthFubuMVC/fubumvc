using System;
using LightningDB;

namespace LightningQueues.Storage.LMDB
{
    public static class LmdbStorageExtensions
    {
        public static QueueConfiguration StoreWithLmdb(this QueueConfiguration configuration, string path)
        {
            return configuration.StoreMessagesWith(new LmdbMessageStore(path));
        }

        public static QueueConfiguration StoreWithLmdb(this QueueConfiguration configuration, string path, EnvironmentConfiguration config)
        {
            return configuration.StoreMessagesWith(new LmdbMessageStore(path, config));
        }

        public static QueueConfiguration StoreWithLmdb(this QueueConfiguration configuration, LightningEnvironment environment)
        {
            return configuration.StoreMessagesWith(new LmdbMessageStore(environment));
        }
    }
}