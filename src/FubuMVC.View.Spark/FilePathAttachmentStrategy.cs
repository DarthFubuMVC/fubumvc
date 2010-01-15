using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using System.Linq;

namespace FubuMVC.View.Spark
{
    public class FilePathAttachmentStrategy : IViewAttachmentStrategy
    {
        public IEnumerable<IViewToken> Find(ActionCall call, ViewBag views)
        {
            return views.Views.Where(v => call.Method.Name == v.Name);
        }
    }
}