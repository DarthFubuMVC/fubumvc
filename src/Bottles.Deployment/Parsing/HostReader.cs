using System.IO;
using Bottles.Configuration;
using FubuCore;
using FubuCore.Util;

namespace Bottles.Deployment.Parsing
{
    public static class HostReader
    {
        public static HostManifest ReadFrom(string fileName, EnvironmentSettings environment, Profile profile)
        {
            var @overrides = new Cache<string, string>();
            environment.Overrides.Each((k,v)=>
            {
                @overrides[k] = v;
            });
            profile.Overrides.Each((k,v)=>
            {
                @overrides[k] = v;
            });

            var parser = new SettingsParser(fileName, @overrides.ToDictionary());
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