using FubuCore;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class WindowsSignInRequest
    {
        private string _url;

        [QueryString]
        public string Url
        {
            get
            {
                if (_url.IsEmpty())
                {
                    _url = "~/";
                }

                return _url;
            }
            set { _url = value; }
        }
    }
}