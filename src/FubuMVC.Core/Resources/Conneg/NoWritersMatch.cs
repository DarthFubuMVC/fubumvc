using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Resources.Conneg
{
    public class NoWritersMatch : LogRecord, DescribesItself
    {
        public static NoWritersMatch For<T>(string acceptType, IEnumerable<IMediaWriter<T>> candidates)
        {
            var match = new NoWritersMatch{
                WriterList = candidates.Select(writer =>
                {
                    return Description.For(writer).Title;
                }).Join(", ")
            };

            match.MimeType = acceptType;

            return match;
        }

        public string MimeType { get; set; }
        public string WriterList { get; set; }

        public void Describe(Description description)
        {
            description.Title = "Conditions were not met for any available writers for mimetype " + MimeType;
            description.ShortDescription = "Candidate writers:  " + WriterList;
        }
    }
}