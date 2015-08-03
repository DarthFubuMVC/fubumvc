using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Services;
using Topshelf;

namespace JasperService
{
    internal static class Program
    {
        public static void Main(params string[] args)
        {
	        var settings = serviceConfiguration();

            HostFactory.Run(x => {
                x.SetServiceName(settings.Name);
                x.SetDisplayName(settings.DisplayName);
                x.SetDescription(settings.Description);

                if (Platform.IsUnix ())
                {
                    x.RunAsPrompt();
                } else
                {
                    x.RunAsLocalService();
                }

				if (settings.UseEventLog)
				{
					x.UseEventLog(settings);
				}

				x.Service<JasperServiceRuntime>(s =>
				{
                    s.ConstructUsing(name => new JasperServiceRuntime(settings));
                    s.WhenStarted(r => r.Start());
                    s.WhenStopped(r => r.Stop());
                    s.WhenPaused(r => r.Stop());
                    s.WhenContinued(r => r.Start());
                    s.WhenShutdown(r => r.Stop());
                });

                x.StartAutomatically();
            });
        }

		private static JasperServiceConfiguration serviceConfiguration()
		{
		    var directory = FubuRuntime.DefaultApplicationPath();
			var fileSystem = new FileSystem();

			return fileSystem.LoadFromFile<JasperServiceConfiguration>(directory, JasperServiceConfiguration.FILE);
		}
    }
}
