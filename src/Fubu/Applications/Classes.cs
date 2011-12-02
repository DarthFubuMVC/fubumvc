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

    

    public class ApplicationRunner : MarshalByRefObject
    {
        private readonly IApplicationSourceFinder _finder;

        public ApplicationRunner() : this(new ApplicationSourceFinder())
        {
        }

        public ApplicationRunner(IApplicationSourceFinder finder)
        {
            _finder = finder;
        }

        public ApplicationStartResponse StartApplication(ApplicationSettings settings)
        {
            var response = new ApplicationStartResponse();

            //try


            /*
             * 1.) find the right application source
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             */
            throw new NotImplementedException();
        }        

        public virtual void StartApplication(IApplicationSource source, ApplicationSettings settings)
        {
            FubuMvcPackageFacility.PhysicalRootPath = settings.GetApplicationFolder();
            var kayakApplication = new FubuKayakApplication(source);

            // Need to make this capture the package registry failures cleanly
            kayakApplication.RunApplication(settings.Port, r => { });
        }

        public virtual IApplicationSource FindSource(ApplicationSettings settings, ApplicationStartResponse theResponse)
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

            var types = _finder.FindApplicationSourceTypes();
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


    public interface IApplicationSourceFinder
    {
        IEnumerable<Type> FindApplicationSourceTypes();
    }

    public class ApplicationSourceFinder : IApplicationSourceFinder
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