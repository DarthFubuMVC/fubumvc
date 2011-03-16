namespace FubuMVC.Core.Security.AntiForgery
{
    public interface IAntiForgerySerializer
    {
        AntiForgeryData Deserialize(string serializedToken);
        string Serialize(AntiForgeryData token);
    }
}