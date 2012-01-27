using System.Web;

namespace FubuMVC.HelloWorld.Services
{
    public interface IHttpSession
    {
        void Clear();
        object this[string key] { get; set; }
    }

    public class CurrentHttpContextSession : IHttpSession
    {
        private readonly HttpContextBase _httpContext;

        public CurrentHttpContextSession(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public void Clear()
        {
            _httpContext.Session.Clear();
        }

        public object this[string key]
        {
            get { return _httpContext.Session [key]; }
            set { _httpContext.Session[key] = value; }
        }
    }
}