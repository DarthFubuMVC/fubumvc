using System.Web;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.WebForms;

namespace FubuMVC.HelloWorld.Controllers.Home
{
    public class HomeController
    {
        private readonly ILogger _log;

        public HomeController(ILogger log)
        {
            _log = log;
        }

        public HomeViewModel Home(HomeInputModel model)
        {
            _log.Debug("Loading home...");
            return new HomeViewModel{
                Text = "Hello, world.",
                CurrentUrl = model.Url
            };
        }

        public HomeViewModel Post(HomeFilesModel model)
        {
            return new HomeViewModel{
                Text = "Hello, world.",
                CurrentUrl = model.Url,
                HomeFile1Present = model.HomeFile1 != null,
                NumberOfFiles = model.Files.Count
            };
        }
    }

    public class HomeFilesModel
    {
        public HttpPostedFileBase HomeFile1 { get; set; }
        public HttpFileCollectionBase Files { get; set; }
        public string Url { get; set; }
    }

    public class HomeViewModel
    {
        public string Text { get; set; }
        public string CurrentUrl { get; set; }
        public bool HomeFile1Present { get; set; }
        public int NumberOfFiles { get; set; }
    }

    public class HomeInputModel
    {
        public string Url { get; set; }
    }

    public class Home : FubuPage<HomeViewModel>
    {
    }
}