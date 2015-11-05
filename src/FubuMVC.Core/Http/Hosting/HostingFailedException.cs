using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using FubuCore;

namespace FubuMVC.Core.Http.Hosting
{
    [Serializable]
    public class HostingFailedException : Exception
    {
        private readonly int _port;
        private readonly string _username;

        public HostingFailedException(Exception innerException, int port) : base(string.Empty, innerException)
        {
            _port = port;
            //Not ideal, but helps identify issues with the specific service account the host may be running under
            var identity = WindowsIdentity.GetCurrent();
            _username = identity == null || identity.Name.IsEmpty() ? "DOMAIN\\user" : identity.Name;
        }

        protected HostingFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message
        {
            get { return @"
To use Katana hosting, you need to either run with administrative rights
or optionally, use 'netsh http add urlacl url=http://*:{0}/ user={1}' at the command line.
See http://msdn.microsoft.com/en-us/library/ms733768.aspx for more information.".ToFormat(_port, _username).Trim(); }
        }
    }
}