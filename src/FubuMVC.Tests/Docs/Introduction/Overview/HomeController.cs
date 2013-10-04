namespace FubuMVC.Tests.Docs.Introduction.Overview
{
    // SAMPLE: overview-homecontroller
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
    // ENDSAMPLE
}