using System.Web;
using HtmlTags;

namespace FubuMVC.Core.SessionState
{
    public interface IFlash
    {
        void Flash(object flashObject);
        T Retrieve<T>();
    }

    public class FlashProvider : IFlash
    {
        public const string FLASH_KEY = "fubuFlash";
        private HttpSessionStateBase _session;

        public HttpSessionStateBase Session { get { return _session ?? (_session = new HttpSessionStateWrapper(HttpContext.Current.Session)); } set { _session = value; } }


        public void Flash(object flashObject)
        {
            string json = JsonUtil.ToJson(flashObject);
            Session[FLASH_KEY] = json;
        }

        public T Retrieve<T>()
        {
            var json = Session[FLASH_KEY] as string;
            Session.Remove(FLASH_KEY);

            return (json != null)
                       ? JsonUtil.Get<T>(json)
                       : default(T);
        }
    }
}