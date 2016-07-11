using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Downloads
{
    public class DownloadFileConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Routes.Resources.Where(x => TypeExtensions.CanBeCastTo<DownloadFileModel>(x.ResourceType()))
                .Each(x => x.AddToEnd(new DownloadFileNode()));
        }
    }
}