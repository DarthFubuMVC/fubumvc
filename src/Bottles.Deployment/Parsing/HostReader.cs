using System.IO;
using Bottles.Configuration;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public static class HostReader
    {
        public static HostManifest ReadFrom(string fileName, EnvironmentSettings environment)
        {
            var parser = new SettingsParser(fileName, environment.Overrides.ToDictionary());
            new FileSystem().ReadTextFile(fileName, parser.ParseText);

            var hostName = Path.GetFileNameWithoutExtension(fileName);
            var host = new HostManifest(hostName);


            var settings = parser.Settings;

            host.RegisterSettings(settings);
            host.RegisterBottles(parser.References);

            return host;
        }
    }
}