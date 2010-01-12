using System.Linq;
using System.Web.UI;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.View
{
    public class FubuMasterPage<TMasterPageViewModel> : MasterPage, IFubuView<TMasterPageViewModel>
        where TMasterPageViewModel : class
    {
        private TMasterPageViewModel _model;

        public TMasterPageViewModel Model
        {
            get
            {
                if (_model != null) return _model;

                var fubuPage = Page as IFubuPage;

                if (fubuPage != null)
                {
                    SetModel(fubuPage.ServiceLocator.GetInstance<IFubuRequest>());
                }

                return _model;
            }
        }

        public void SetModel(IFubuRequest request)
        {
            _model = request.Find<TMasterPageViewModel>().FirstOrDefault();
        }
    }

    public class FubuMasterPage : MasterPage, IFubuView
    {
    }
}