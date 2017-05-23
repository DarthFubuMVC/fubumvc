using System.Text;

namespace LightningQueues.Net.Protocol.V1
{
    public static class Constants
    {
        public const string Received = "Recieved";
        public const string SerializationFailure = "FailDesr";
        public const string ProcessingFailure = "FailPrcs";
        public const string Acknowledged = "Acknowledged";
        public const string Revert = "Revert";
        public const string QueueDoesNotExist = "Qu-Exist";
        
        public static byte[] ReceivedBuffer = Encoding.Unicode.GetBytes(Received);
        public static byte[] AcknowledgedBuffer = Encoding.Unicode.GetBytes(Acknowledged);
        public static byte[] RevertBuffer = Encoding.Unicode.GetBytes(Revert);
        public static byte[] QueueDoesNotExistBuffer = Encoding.Unicode.GetBytes(QueueDoesNotExist);
        public static byte[] SerializationFailureBuffer = Encoding.Unicode.GetBytes(SerializationFailure);
        public static byte[] ProcessingFailureBuffer = Encoding.Unicode.GetBytes(ProcessingFailure);
    }
}
