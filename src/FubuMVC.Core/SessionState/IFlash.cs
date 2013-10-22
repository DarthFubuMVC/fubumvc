using FubuMVC.Core.Runtime;
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

        public FlashProvider()
        {
        }

        public FlashProvider(ISessionState sessionState)
        {
            Session = sessionState;
        }

        public ISessionState Session { get; set; }

        public void Flash(object flashObject)
        {
            string json = JsonUtil.ToJson(flashObject);
            Session.Set(FLASH_KEY, json);
        }

        public T Retrieve<T>()
        {
            var json = Session.Get<string>(FLASH_KEY);
            Session.Remove(FLASH_KEY);

            return (json != null)
                       ? JsonUtil.Get<T>(json)
                       : default(T);
        }
    }
}