using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Registration.Diagnostics;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewAttachmentLog : NodeEvent
    {
        private readonly IViewProfile _profile;
        private readonly IList<ViewsForFilterLog> _logs = new List<ViewsForFilterLog>();
        

        public ViewAttachmentLog(IViewProfile profile)
        {
            _profile = profile;
        }

        public void FoundViews(IViewsForActionFilter filter, IEnumerable<IViewToken> views)
        {
            _logs.Add(new ViewsForFilterLog(filter, views));
        }

        public IViewProfile Profile
        {
            get { return _profile; }
        }

        public IEnumerable<ViewsForFilterLog> Logs
        {
            get { return _logs; }
        }
    }
}