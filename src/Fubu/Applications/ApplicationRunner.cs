using System;
using System.Collections.Generic;
using System.Threading;
using Bottles;
using FubuKayak;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;

namespace Fubu.Applications
{
    public class ApplicationRunner : MarshalByRefObject, IDisposable
    {
        private readonly IApplicationSourceFinder _sourceFinder;
        private FubuKayakApplication _kayakApplication;

        public ApplicationRunner() : this(new ApplicationSourceFinder(new ApplicationSourceTypeFinder()))
        {
        }

        public ApplicationRunner(IApplicationSourceFinder sourceFinder)
        {
            _sourceFinder = sourceFinder;
        }

        public ApplicationStartResponse StartApplication(ApplicationSettings settings, ManualResetEvent reset)
        {
            var response = new ApplicationStartResponse();

            try
            {
                var source = _sourceFinder.FindSource(settings, response);
                if (source == null)
                {
                    response.Status = ApplicationStartStatus.CouldNotResolveApplicationSource;
                }
                else
                {
                    StartApplication(source, settings, reset);
                    response.ApplicationSourceName = source.GetType().AssemblyQualifiedName;

                    reset.WaitOne();
                    determineBottleFolders(response);
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.ToString();
                response.Status = ApplicationStartStatus.ApplicationSourceFailure;
            }

            return response;
        }

        public virtual void StartApplication(IApplicationSource source, ApplicationSettings settings, ManualResetEvent reset)
        {
            FubuMvcPackageFacility.PhysicalRootPath = settings.GetApplicationFolder();
            _kayakApplication = new FubuKayakApplication(source);
// Put a thread here

            ThreadPool.QueueUserWorkItem(o =>
            {
                // Need to make this capture the package registry failures cleanly
                _kayakApplication.RunApplication(settings.Port, r => reset.Set());
            });


        }

        private static void determineBottleFolders(ApplicationStartResponse response)
        {
            var list = new List<string>();
            PackageRegistry.Packages.Each(pak =>
            {
                pak.ForFolder(BottleFiles.WebContentFolder, list.Add);
                pak.ForFolder(BottleFiles.BinaryFolder, list.Add);
                pak.ForFolder(BottleFiles.DataFolder, list.Add);
            });

            response.BottleDirectories = list.ToArray();
        }

        public RecycleResponse Recycle()
        {
            try
            {
                _kayakApplication.Recycle(r => { });
                return new RecycleResponse{
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new RecycleResponse{
                    Success = false,
                    Message = e.ToString()
                };
            }
        }




        public void Dispose()
        {
            _kayakApplication.Stop();
        }
    }
}