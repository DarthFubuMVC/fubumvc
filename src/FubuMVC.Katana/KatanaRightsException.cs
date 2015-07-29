using System;
using System.Runtime.Serialization;

namespace FubuMVC.Katana
{
    [Serializable]
    public class KatanaRightsException : Exception
    {
        public KatanaRightsException(Exception innerException) : base(string.Empty, innerException)
        {
        }

        protected KatanaRightsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message
        {
            get { return @"
To use Katana hosting, you need to either run with administrative rights
or optionally, use 'netsh http add urlacl url=http://+:80/MyUri user=DOMAIN\\user' at the command line.
See http://msdn.microsoft.com/en-us/library/ms733768.aspx for more information.
".Trim(); }
        }
    }
}