using FubuMVC.WebForms;

namespace FubuMVC.GuideApp.Controllers.Home
{
    public class HomeController
    {
        public HomeViewModel Home(HomeInputModel inputModel)
        {
            return new HomeViewModel();
        }
    }

    public class HomeInputModel
    {
    }

    public class HomeViewModel
    {
    }

    public class HomeView : FubuPage<HomeViewModel>
    {
    }
}
