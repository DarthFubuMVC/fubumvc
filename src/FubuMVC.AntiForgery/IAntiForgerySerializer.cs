namespace FubuMVC.AntiForgery
{
    public interface IAntiForgerySerializer
    {
        AntiForgeryData Deserialize(string serializedToken);
        string Serialize(AntiForgeryData token);
    }
}