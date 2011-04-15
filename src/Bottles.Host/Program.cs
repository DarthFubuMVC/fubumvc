using System;
using Bottle.Host;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;
using Topshelf;

namespace Bottles.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(h =>
                                {
                                    h.SetDescription("Bottle Host");
                                    h.SetServiceName("bottle-host");
                                    h.SetDisplayName("display");

                                    h.Service<BottleHost>(c =>
                                                              {
                                                                  c.ConstructUsing(n =>
                                                                                       {
                                                                                           var fileSystem = new FileSystem();
                                                                                           var packageExploder = new PackageExploder(new ZipFileService(), new PackageExploderLogger(Console.WriteLine), fileSystem);
                                                                                           return new BottleHost(packageExploder, fileSystem);
                                                                                       });
                                                                  c.WhenStarted(s => s.Start());
                                                                  c.WhenStopped(s => s.Stop());
                                                              });
                                });
        }
    }
}
