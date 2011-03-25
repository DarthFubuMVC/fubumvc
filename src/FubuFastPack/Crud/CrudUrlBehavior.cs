using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuFastPack.Crud
{
    public class CrudUrlBehavior : BasicBehavior
    {
        private readonly IUrlRegistry _urls;
        private readonly IFubuRequest _request;

        public CrudUrlBehavior(IUrlRegistry urls, IFubuRequest request)
            : base(PartialBehavior.Ignored)
        {
            _urls = urls;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var model = _request.Find<EditEntityModel>().Single();
            if (model.Target.IsNew())
            {
                model.SubmitAction = _urls.UrlFor(model);
            }
            else
            {
                model.PropertyUpdateUrl = _urls.UrlForPropertyUpdate(model.EntityType);
            }

            return DoNext.Continue;
        }
    }
}