using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using LightningQueues.Model;

namespace LightningQueues.Protocol
{
    public static class SerializationExtensions
    {
        public static string ToQueryString(this NameValueCollection qs)
        {
            return string.Join("&", Array.ConvertAll(qs.AllKeys, key => string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(qs[key]))));
        }

        public static Message[] ToMessages(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            using (var br = new BinaryReader(ms))
            {
                var numberOfMessages = br.ReadInt32();
                var msgs = new Message[numberOfMessages];
                for (int i = 0; i < numberOfMessages; i++)
                {
                    msgs[i] = new Message
                    {
                        Id = new MessageId
                        {
                            SourceInstanceId = new Guid(br.ReadBytes(16)),
                            MessageIdentifier = new Guid(br.ReadBytes(16))
                        },
                        Queue = br.ReadString(),
                        SubQueue = br.ReadString(),
                        SentAt = DateTime.FromBinary(br.ReadInt64()),
                    };
                    var headerCount = br.ReadInt32();
                    msgs[i].Headers = new NameValueCollection(headerCount);
                    for (var j = 0; j < headerCount; j++)
                    {
                        msgs[i].Headers.Add(
                            br.ReadString(),
                            br.ReadString()
                            );
                    }
                    var byteCount = br.ReadInt32();
                    msgs[i].Data = br.ReadBytes(byteCount);
                    if (string.IsNullOrEmpty(msgs[i].SubQueue))
                        msgs[i].SubQueue = null;
                }
                return msgs;
            }
        }

        public static byte[] Serialize(this Message[] messages)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(messages.Length);
                foreach (var message in messages)
                {
                    writer.Write(message.Id.SourceInstanceId.ToByteArray());
                    writer.Write(message.Id.MessageIdentifier.ToByteArray());
                    writer.Write(message.Queue);
                    writer.Write(message.SubQueue ?? "");
                    writer.Write(message.SentAt.ToBinary());

                    writer.Write(message.Headers.Count);
                    foreach (string key in message.Headers)
                    {
                        writer.Write(key);
                        writer.Write(message.Headers[key]);
                    }

                    writer.Write(message.Data.Length);
                    writer.Write(message.Data);
                }
                writer.Flush();
                return stream.ToArray();
            }
        }

    }
}