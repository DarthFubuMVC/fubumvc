using System.Web.Script.Serialization;

namespace FubuMVC.Core.Util
{
    public static class JsonUtil
    {
#pragma warning disable 618,612
        public static string ToJson(object objectToSerialize)
        {
            return new JavaScriptSerializer().Serialize(objectToSerialize);
        }

        public static T Get<T>(string rawJson)
        {
            return new JavaScriptSerializer().Deserialize<T>(rawJson);
        }

        public static object Get(string rawJson)
        {
            return new JavaScriptSerializer().DeserializeObject(rawJson);
        }
#pragma warning restore 618,612
    }
}