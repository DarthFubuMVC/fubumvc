using System.Text;

namespace LightningQueues.Protocol
{
    public static class ProtocolConstants
    {
        public const string Recieved = "Recieved";
        public const string SerializationFailure = "FailDesr";
        public const string ProcessingFailure = "FailPrcs";
        public const string Acknowledged = "Acknowledged";
        public const string Revert = "Revert";
        public const string QueueDoesNotExists = "Qu-Exist";
        
        public static byte[] RecievedBuffer = Encoding.Unicode.GetBytes(Recieved);
        public static byte[] AcknowledgedBuffer = Encoding.Unicode.GetBytes(Acknowledged);
        public static byte[] RevertBuffer = Encoding.Unicode.GetBytes(Revert);
        public static byte[] QueueDoesNoExiststBuffer = Encoding.Unicode.GetBytes(QueueDoesNotExists);
        public static byte[] SerializationFailureBuffer = Encoding.Unicode.GetBytes(SerializationFailure);
        public static byte[] ProcessingFailureBuffer = Encoding.Unicode.GetBytes(ProcessingFailure);
    }
}
