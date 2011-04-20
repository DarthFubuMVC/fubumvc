using System.IO;
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

            host.RegisterSettings(parser.Settings);
            host.RegisterBottles(parser.References);

            return host;
        }
    }
}