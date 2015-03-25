using System;

namespace Serenity.WebDriver.EmbeddedDrivers
{
    public interface IEmbeddedDriver
    {
        Version Version { get; }
        string ResourceName { get; }
        string ExtractedFileName { get; }
    }
}