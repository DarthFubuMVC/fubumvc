using FUBUPROJECTNAME.Models.View;

namespace FUBUPROJECTNAME.Controllers.Home
{
    public class HomeController
    {
        public WelcomeViewModel Welcome()
        {
            return new WelcomeViewModel();
        }
    }
}