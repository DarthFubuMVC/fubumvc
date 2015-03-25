namespace Serenity.WebDriver.EmbeddedDrivers
{
    public interface IEmbeddedDriverExtractor<TEmbeddedDriver>
    {
        bool ShouldExtract();
        void Extract();
    }
}