using System.Web;

namespace FubuMVC.HelloFubuSpark.Services
{
    public interface IHttpSession
    {
        void Clear();
        object this[string key] { get; set; }
    }

    public class CurrentHttpContextSession : IHttpSession
    {
        public void Clear()
        {
            HttpContext.Current.Session.Clear();
        }

        public object this[string key]
        {
            get { return HttpContext.Current.Session [key]; }
            set { HttpContext.Current.Session[key] = value; }
        }
    }
}