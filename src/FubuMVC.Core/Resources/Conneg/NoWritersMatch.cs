using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Resources.Conneg
{
    public class NoWritersMatch : LogRecord, DescribesItself
    {
        public static NoWritersMatch For<T>(string acceptType, IEnumerable<IMedia<T>> candidates)
        {
            var match = new NoWritersMatch{
                WriterList = candidates.Select(writer =>
                {
                    var title = Description.For(writer).Title;
                    var condition = Description.For(writer.Condition).Title;

                    return "{0} ({1})".ToFormat(title, condition);
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