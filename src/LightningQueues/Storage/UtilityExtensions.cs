using System.Collections.Generic;
using System.IO;

namespace LightningQueues.Storage
{
    public static class UtilityExtensions
    {
        public static byte[] ToBytes(this IDictionary<string, string> queryString)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(queryString.Count);
                foreach (var entry in queryString)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value);
                }
                return ms.ToArray();
            }
        }

        public static IDictionary<string, string> ToDictionary(this byte[] bytes)
        {
            var dictionary = new Dictionary<string, string>();
            using (var ms = new MemoryStream(bytes))
            using (var reader = new BinaryReader(ms))
            {
                var count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    var key = reader.ReadString();
                    var value = reader.ReadString();
                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }

        public static QueueConfiguration UseNoStorage(this QueueConfiguration config)
        {
            return config.StoreMessagesWith(new NoStorage());
        }
    }
}