using System.Web;
using FubuMVC.Core.View;
using FubuMVC.WebForms;

namespace FubuMVC.HelloWorld.Controllers.Home
{
    public class HomeController
    {
        public HomeViewModel Home(HomeInputModel model)
        {
            return new HomeViewModel
            {
                Text = "Hello, world.",
                CurrentUrl = model.Url
            };
        }

        public HomeViewModel Post(HomeFilesModel model)
        {
            return new HomeViewModel
            {
                Text = "Hello, world.",
                CurrentUrl = model.Url,
                NumberOfFiles = model.Files == null ? 0 : model.Files.Count
            };
        }
    }

    public class HomeFilesModel
    {
        public HttpFileCollectionWrapper Files { get; set; }
        public string Url { get; set; }
    }

    public class HomeViewModel
    {
        public string Text { get; set; }
        public string CurrentUrl { get; set; }
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