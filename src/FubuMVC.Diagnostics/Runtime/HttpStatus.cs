using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuMVC.Core.Runtime.Logging;

namespace FubuMVC.Diagnostics.Runtime
{
    public class HttpStatus
    {
        private readonly HttpStatusCode _status;
        private readonly string _description;

        public HttpStatus(HttpStatusReport report, bool failed)
        {
            if(report != null)
            {
                _status = report.Status;
                _description = report.Description;
            }
            else
            {
                _status = failed ? HttpStatusCode.InternalServerError : HttpStatusCode.OK;
            }
           
            if (_description.IsEmpty())
            {
                _description = _status.ToString().SplitPascalCase();
            }
        }

        public HttpStatusCode Status
        {
            get { return _status; }
        }

        public string Description
        {
            get { return _description; }
        }

        public override string ToString()
        {
            return _description.IsEmpty()
                       ? _status.As<int>().ToString()
                       : "{0} {1}".ToFormat(_status.As<int>(), _description);
        }
    }
}