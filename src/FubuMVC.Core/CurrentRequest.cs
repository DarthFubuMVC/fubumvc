using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core
{
    [Obsolete("Humongous PITA, Jeremy needs to provide instructions on what to do with this instead")]
    public class CurrentRequest
    {
        //public string[] AcceptTypes{ get; set;}
        public string ApplicationPath { get; set; }
        public string AppRelativeCurrentExecutionFilePath { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public string CurrentExecutionFilePath { get; set; }
        public string FilePath { get; set; }
        public string HttpMethod { get; set; }
        public string IsAuthenticated { get; set; }
        public string IsLocal { get; set; }
        public string IsSecureConnection { get; set; }
        public string Path { get; set; }
        public string PathInfo { get; set; }
        public string PhysicalApplicationPath { get; set; }
        public string PhysicalPath { get; set; }
        public string RawUrl { get; set; }
        public string RequestType { get; set; }
        public int TotalBytes { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string UserAgent { get; set; }
        public string UserHostAddress { get; set; }
        public string UserHostName { get; set; }

        public string RequestedMimeType()
        {
            return ContentType == null 
                ? string.Empty 
                : ContentType.Split(';').First();
        }

        public bool MatchesOneOfTheseMimeTypes(params string[] mimeTypes)
        {
            return mimeTypes.Contains(RequestedMimeType());
        }
    }
}