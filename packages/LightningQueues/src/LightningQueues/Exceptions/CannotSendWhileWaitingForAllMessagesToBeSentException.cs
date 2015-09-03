using System;
using System.Runtime.Serialization;

namespace LightningQueues.Exceptions
{
    [Serializable]
	public class CannotSendWhileWaitingForAllMessagesToBeSentException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public CannotSendWhileWaitingForAllMessagesToBeSentException()
		{
		}

		public CannotSendWhileWaitingForAllMessagesToBeSentException(string message) : base(message)
		{
		}

		public CannotSendWhileWaitingForAllMessagesToBeSentException(string message, Exception inner) : base(message, inner)
		{
		}

		protected CannotSendWhileWaitingForAllMessagesToBeSentException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}