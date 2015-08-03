using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.Http.Hosting
{
    [Serializable]
    public class AdminRightsException : Exception
    {
        public AdminRightsException(Exception innerException) : base(string.Empty, innerException)
        {
        }

        protected AdminRightsException(SerializationInfo info, StreamingContext context) : base(info, context)
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