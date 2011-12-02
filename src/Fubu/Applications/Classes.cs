using System;
using System.Collections.Generic;
using FubuKayak;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuCore;
using System.Linq;

namespace Fubu.Applications
{
    public enum ApplicationStartStatus
    {
        Started,
        ApplicationSourceFailure,
        CouldNotResolveApplicationSource
    }

    [Serializable]
    public class ApplicationStartResponse
    {
        public ApplicationStartStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public string[] ApplicationSourceTypes { get; set; }
    }

    [Serializable]
    public class RecycleResponse
    {
        
    }

    public class ApplicationRunner : MarshalByRefObject
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

        public ApplicationStartResponse StartApplication(ApplicationSettings settings)
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
                    StartApplication(source, settings);
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.ToString();
                response.Status = ApplicationStartStatus.ApplicationSourceFailure;
            }

            return response;
        }

        public RecycleResponse Recycle()
        {
            throw new NotImplementedException();
            //_kayakApplication.Recycle(r => { });
        }

        public virtual void StartApplication(IApplicationSource source, ApplicationSettings settings)
        {
            FubuMvcPackageFacility.PhysicalRootPath = settings.GetApplicationFolder();
            _kayakApplication = new FubuKayakApplication(source);

            // Need to make this capture the package registry failures cleanly
            _kayakApplication.RunApplication(settings.Port, r => { });
        }


    }

    public interface IApplicationSourceFinder
    {
        IApplicationSource FindSource(ApplicationSettings settings, ApplicationStartResponse theResponse);
    }

    public class ApplicationSourceFinder : IApplicationSourceFinder
    {
        private readonly IApplicationSourceTypeFinder _typeFinder;

        public ApplicationSourceFinder(IApplicationSourceTypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public IApplicationSource FindSource(ApplicationSettings settings, ApplicationStartResponse theResponse)
        {
            Type type = findType(settings, theResponse);
            return type == null ? null : (IApplicationSource)Activator.CreateInstance(type);
        }

        private Type findType(ApplicationSettings settings, ApplicationStartResponse theResponse)
        {
            if (settings.ApplicationSourceName.IsNotEmpty())
            {
                return Type.GetType(settings.ApplicationSourceName);
            }

            var types = _typeFinder.FindApplicationSourceTypes();
            theResponse.ApplicationSourceTypes = types.Select(x => x.AssemblyQualifiedName).ToArray();

            if (!types.Any()) return null;

            return only(types) ?? matchingTypeName(settings, types);
        }

        private static Type only(IEnumerable<Type> types)
        {
            return types.Count() == 1 ? types.Single() : null;
        }

        private static Type matchingTypeName(ApplicationSettings settings, IEnumerable<Type> types)
        {
            return types.FirstOrDefault(x => x.Name == settings.Name);
        }
    }

    public interface IApplicationSourceTypeFinder
    {
        IEnumerable<Type> FindApplicationSourceTypes();
    }

    public class ApplicationSourceTypeFinder : IApplicationSourceTypeFinder
    {
        public IEnumerable<Type> FindApplicationSourceTypes()
        {
            throw new NotImplementedException();
        }

        public IApplicationSource CreateSource(string typeName)
        {
            throw new NotImplementedException();
        }

        public IApplicationSource CreateSource(Type type)
        {
            throw new NotImplementedException();
        }
    }

   
}