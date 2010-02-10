using System;

namespace FubuMVC.Core.Configuration
{
    public interface ISettingsProvider
    {
        T SettingsFor<T>() where T : class, new();
        object SettingsFor(Type settingsType);
    }
}